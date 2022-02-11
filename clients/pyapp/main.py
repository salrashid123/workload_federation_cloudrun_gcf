#!/usr/bin/python
from google.auth import credentials
from google.cloud import  iam_credentials_v1

import google.auth
import google.oauth2.credentials

from google.auth.transport.requests import AuthorizedSession, Request

url = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app/dump"
aud = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app"
service_account = 'oidc-federated@mineral-minutia-820.iam.gserviceaccount.com'

client = iam_credentials_v1.services.iam_credentials.IAMCredentialsClient()

name = "projects/-/serviceAccounts/{}".format(service_account)
id_token = client.generate_id_token(name=name,audience=aud, include_email=True)

print(id_token.token)

creds = google.oauth2.credentials.Credentials(id_token.token)
authed_session = AuthorizedSession(creds)
r = authed_session.get(url)
print(r.status_code)
print(r.text)

