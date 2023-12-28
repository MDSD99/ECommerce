using System.Globalization;
using System.IO.Compression;
using CatalogService.Api.Core.Domain;
using Microsoft.Data.SqlClient;
using Polly;

#nullable disable
namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
        {
            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(
                retryCount:3,
                sleepDurationProvider:retry=>TimeSpan.FromSeconds(5),
                onRetry: (exception,timespan,retry,ctx) =>
                {
                    logger.LogWarning(exception,"Exception {ExceptionType} with message {Message} detected on attempt {retry}",retry);
                });

            var setupDirPath = Path.Combine(env.ContentRootPath,"Infrastructure","Setup","SeedFiles");

            var picturePath = "";

            await policy.ExecuteAsync(() =>  ProcessSeeding(context, setupDirPath, picturePath, logger) );
        }

        public async Task ProcessSeeding(CatalogContext context,string setupDirPath,string picturePath,ILogger logger)
        {
            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));

                await context.SaveChangesAsync();
            }
            if (!context.CatalogTypes.Any())
            {
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));

                await context.SaveChangesAsync();
            }
            if (!context.CatalogItems.Any())
            {
                await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath,context));

                await context.SaveChangesAsync();

                GetCatalogItemPictures(setupDirPath,picturePath);
            }
        }

        private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
        {
            IEnumerable<CatalogBrand> GetPreConfiguredBrands()
            {
                return new List<CatalogBrand>()
                {
                    new CatalogBrand(){Brand="Azure"},
                    new CatalogBrand(){Brand=".NET"},
                    new CatalogBrand(){Brand="Visual Studio"},
                    new CatalogBrand(){Brand="SQL Server"},
                    new CatalogBrand(){Brand="Other"},
                };
            }

            string fileName = Path.Combine(contentPath, "CatalogBrands.txt");

            if (!File.Exists(fileName))
            {
                return GetPreConfiguredBrands();
            }

            var fileContent = File.ReadAllLines(fileName);

            var list = fileContent.Select(s => new CatalogBrand() { Brand = s.Trim('"') });

            return list ?? GetPreConfiguredBrands();
        }

        private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath)
        {
            IEnumerable<CatalogType> GetPreConfiguredTypes()
            {
                return new List<CatalogType>()
                {
                    new CatalogType(){Type="Mug"},
                    new CatalogType(){Type="T-Shirt"},
                    new CatalogType(){Type="Sheet"},
                    new CatalogType(){Type="USB Memory Stick"},
                };
            }

            string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

            if (!File.Exists(fileName))
            {
                return GetPreConfiguredTypes();
            }

            var fileContent = File.ReadAllLines(fileName);

            var list = fileContent.Select(s => new CatalogType() { Type = s.Trim('"') });

            return list??GetPreConfiguredTypes();
        }

        private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
        {
            IEnumerable<CatalogItem> GetPreConfiguredItems()
            {
                return new List<CatalogItem>()
                {
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=2,AvaibleStock=100,Description=".NET Bot Black Hoodie,and more",Name="NET Bot Black Hoodie",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=1,CatalogBrandId=2,AvaibleStock=100,Description="NET Black & White Mug",Name="NET Black & White Mug",PictureFileName="19.5,1-png",OnReorder=true},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=5,AvaibleStock=100,Description="Prism White T-Shirt",Name="Prism White T-Shirt",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=2,AvaibleStock=100,Description="NET Foundation T-shirt",Name="NET Foundation T-shirt",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=3,CatalogBrandId=5,AvaibleStock=100,Description="Roslyn Red Sheet",Name="Roslyn Red Sheet",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=2,AvaibleStock=100,Description=".NET Blue Hoodie",Name=".NET Blue Hoodie",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=5,AvaibleStock=100,Description="Roslyn Red T-Shirt",Name="Roslyn Red T-Shirt",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=5,AvaibleStock=100,Description="Kudu Purple Hoodie",Name="Kudu Purple Hoodie",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=1,CatalogBrandId=5,AvaibleStock=100,Description="Cup<T> White Mug,",Name="Cup<T> White Mug,",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=3,CatalogBrandId=2,AvaibleStock=100,Description="NET Foundation Sheet",Name="NET Foundation Sheet",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=3,CatalogBrandId=2,AvaibleStock=100,Description="Cup<T> Sheet",Name="Cup<T> Sheet",PictureFileName="19.5,1-png",OnReorder=false},
                    new CatalogItem{ CatalogTypeId=2,CatalogBrandId=5,AvaibleStock=100,Description="Prism White TShirt",Name="Prism White TShirt",PictureFileName="19.5,1-png",OnReorder=false},

                };
            }
            string fileName = Path.Combine(contentPath, "CatalogItems.txt");

            if (!File.Exists(fileName))
            {
                return GetPreConfiguredItems();
            }

            var catalogtypeldLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

            var fileContent = File.ReadAllLines(fileName).Skip(1).Select(i => i.Split(',')).Select(i => new CatalogItem()
            {
                CatalogTypeId = (int)catalogtypeldLookup[i[0]],
                CatalogBrandId = (int)catalogtypeldLookup[i[1]],
                Description = i[2].Trim('"').Trim(),
                Name = i[3].Trim('"').Trim(),
                Price = Decimal.Parse(i[4].Trim('"').Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                PictureFileName = i[5].Trim('"').Trim(),
                AvaibleStock = string.IsNullOrEmpty(i[6]) ? 0 : int.Parse(i[6]),
                OnReorder = Convert.ToBoolean(i[7])
            });

            return fileContent;
        }

        private void GetCatalogItemPictures(string contentPath,string picturePath)
        {
            picturePath ??= "Pics";

            if(picturePath!=null)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(picturePath);

                foreach(FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                string zipCatalogItemPictures = Path.Combine(contentPath,"CatalogItems.zip");

                ZipFile.ExtractToDirectory(zipCatalogItemPictures,picturePath);
            }
        }
    }
}

