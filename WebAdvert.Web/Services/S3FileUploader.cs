using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebAdvert.Web.Interfaces;

namespace WebAdvert.Web.Services
{
    public class S3FileUploader : IFileUploader
    {
        private readonly IConfiguration configuration;

        public S3FileUploader(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream storageStream)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(message: "File name must be specified");
            }
            var bucketName = configuration.GetValue<string>("ImageBucket");
            var result = false;
            using (var client = new AmazonS3Client())
            {
                if (storageStream.Length > 0)
                {
                    if (storageStream.CanSeek)
                    {
                        storageStream.Seek(0, SeekOrigin.Begin);
                    }
                    var request = new PutObjectRequest
                    {
                        AutoCloseStream = true,
                        BucketName = bucketName,
                        InputStream = storageStream,
                        Key = fileName
                    };
                    var response = await client.PutObjectAsync(request);
                    result = response.HttpStatusCode == HttpStatusCode.OK;
                }
            }
            return result;
        }
    }
}
