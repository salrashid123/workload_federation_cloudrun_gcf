package com.test;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

import com.google.cloud.iam.credentials.v1.GenerateIdTokenRequest;
import com.google.cloud.iam.credentials.v1.GenerateIdTokenResponse;
import com.google.cloud.iam.credentials.v1.IamCredentialsClient;
import com.google.cloud.iam.credentials.v1.ServiceAccountName;

public class TestApp {
	public static void main(String[] args) {
		TestApp tc = new TestApp();
	}

	private static String url = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app/dump";
	private static String aud = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app";
	private static String serviceAccount = "oidc-federated@mineral-minutia-820.iam.gserviceaccount.com";

	public TestApp() {
		try {

			// mvn -D"GOOGLE_APPLICATION_CREDENTIALS=/tmp/sts-creds.json" clean install exec:java -q
			
			IamCredentialsClient iamCredentialsClient = IamCredentialsClient.create();

			String name = ServiceAccountName.of("-", serviceAccount).toString();

			GenerateIdTokenRequest idrequest = GenerateIdTokenRequest.newBuilder().setName(name).setAudience(aud)
					.setIncludeEmail(true).build();
			GenerateIdTokenResponse idresponse = iamCredentialsClient.generateIdToken(idrequest);
			System.out.println("IDToken " + idresponse.getToken());

			URL u = new URL(url);
			HttpURLConnection conn = (HttpURLConnection) u.openConnection();

			conn.setRequestProperty("Authorization", "Bearer " + idresponse.getToken());
			conn.setRequestMethod("GET");

			System.out.println("Response Code: " + conn.getResponseCode());

		} catch (Exception ex) {
			System.out.println("Error:  " + ex.getMessage());
		}
	}

}
