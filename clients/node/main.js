
const { IAMCredentialsClient } = require('@google-cloud/iam-credentials');
const axios = require('axios');

async function main() {

  const url = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app/dump";
  const aud = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app";
  const serviceAccount = "oidc-federated@mineral-minutia-820.iam.gserviceaccount.com";


  const iam_client = new IAMCredentialsClient();
  const [resp] = await iam_client.generateIdToken({
    name: `projects/-/serviceAccounts/${serviceAccount}`,
    audience: aud,
    includeEmail: true
  });
  console.info(resp.token);

  axios.get(url, {
    headers: {
      'Authorization': `Bearer ${resp.token}`
    }
  })
    .then((res) => {
      console.log(res.data)
    })
    .catch((error) => {
      console.error(error)
    })
}

main().catch(console.error);