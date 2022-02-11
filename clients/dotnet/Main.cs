using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth;
using Google.Apis.Http;
using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using System.Net.Http.Headers;
using Google.Cloud.Iam.Credentials.V1;

//  ERROR: Error reading credential file from location /tmp/sts-creds.json: Error creating credential from JSON. Unrecognized credential type external_account.
//   Please check the value of the Environment Variable GOOGLE_APPLICATION_CREDENTIALS

namespace Program
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + err.Message);
                }
            }
        }

        public async Task<string> Run()
        {

            string url = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app/dump";
            string aud = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app";
            string serviceAccount = "oidc-federated@mineral-minutia-820.iam.gserviceaccount.com";

            GoogleCredential sourceCredential = await GoogleCredential.GetApplicationDefaultAsync();

            IAMCredentialsClient client = IAMCredentialsClient.Create();
            GenerateIdTokenResponse resp = client.GenerateIdToken(new GenerateIdTokenRequest()
            {
                Name = "projects/-/serviceAccounts/" + serviceAccount,
                Audience = aud,
                IncludeEmail = true
            });

            Console.WriteLine("ID Token " + resp.Token);

            using (var httpClient = new HttpClient())
            {               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resp.Token);
                string response = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                Console.WriteLine(response);
                return response;
            }
        }
    }
}

