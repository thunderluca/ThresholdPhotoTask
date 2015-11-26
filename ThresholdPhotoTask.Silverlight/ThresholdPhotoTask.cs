using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

namespace ThresholdPhotoTask
{
    public class PhotoTask
    {
        #region Constructor

        public PhotoTask(StorageFile destinationFile)
        {
            OutputFile = destinationFile;
        }

        #endregion

        #region Methods

        public static async Task<bool> IsWindowsTen()
        {
            return Environment.OSVersion.Version.Major >= 10;
        }

        public async Task<StorageFile> LaunchAsync()
        {
            if (CropWidthPixels <= 0 || CropHeightPixels <= 0)
            {
                throw new ArgumentException("Cannot crop an image with zero or null dimension",
                    CropWidthPixels <= 0 ? "CropWidthPixels" : "CropHeightPixels");
            }

            var storageAssembly = typeof(Windows.ApplicationModel.DataTransfer.DataPackage).GetTypeInfo().Assembly;

            var storageManagerType =
                storageAssembly.GetType("Windows.ApplicationModel.DataTransfer.SharedStorageAccessManager");

            var token =
                storageManagerType.GetTypeInfo()
                    .DeclaredMethods.FirstOrDefault(m => m.Name == "AddFile")
                    .Invoke(storageManagerType, new object[] { OutputFile });

            var parameters = new ValueSet
            {
                {"CropWidthPixels", CropWidthPixels},
                {"CropHeightPixels", CropHeightPixels},
                {"EllipticalCrop", EllipticalCrop},
                {"ShowCamera", ShowCamera},
                {"DestinationToken", token}
            };

            var launcherAssembly = typeof(Launcher).GetTypeInfo().Assembly;

            var optionsType = launcherAssembly.GetType("Windows.System.LauncherOptions");

            var options = Activator.CreateInstance(optionsType);

            var targetProperty = options.GetType().GetRuntimeProperty("TargetApplicationPackageFamilyName");

            targetProperty.SetValue(options, "Microsoft.Windows.Photos_8wekyb3d8bbwe");

            var launcherType = launcherAssembly.GetType("Windows.System.Launcher");

            var launchUriResult = launcherAssembly.GetType("Windows.System.LaunchUriResult");

            var asTask = GetAsTask();

            var t = asTask.MakeGenericMethod(launchUriResult);

            var method = launcherType.GetTypeInfo()
                .DeclaredMethods.FirstOrDefault(
                    m => m.Name == "LaunchUriForResultsAsync" && m.GetParameters().Length == 3);

            var mt = method.Invoke(launcherType,
                new[] { new Uri("microsoft.windows.photos.crop:"), options, parameters });

            var task = t.Invoke(launcherType, new[] { mt });

            var result = await (dynamic)task;

            string statusStr = result.Status.ToString();

            return statusStr.Contains("Success") ? OutputFile : null;
        }

        private static MethodInfo GetAsTask()
        {
            var methods = typeof(WindowsRuntimeSystemExtensions).GetRuntimeMethods();
            return
                methods.Where(method => method.Name == "AsTask")
                    .FirstOrDefault(
                        method => method.GetParameters().Any(boh => boh.ParameterType.Name == "IAsyncOperation`1"));
        }

        #endregion

        #region Properties

        public StorageFile OutputFile { get; }

        public int CropWidthPixels { private get; set; }

        public int CropHeightPixels { private get; set; }

        public bool EllipticalCrop { private get; set; }

        public bool ShowCamera { private get; set; }

        #endregion
    }
}
