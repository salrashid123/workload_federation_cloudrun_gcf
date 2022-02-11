package main

import (
	"context"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"

	credentials "cloud.google.com/go/iam/credentials/apiv1"
	credentialspb "google.golang.org/genproto/googleapis/iam/credentials/v1"
)

var ()

const (
	url            = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app/dump"
	aud            = "https://federated-auth-cloud-run-6w42z6vi3q-uc.a.run.app"
	serviceAccount = "oidc-federated@mineral-minutia-820.iam.gserviceaccount.com"
)

func main() {
	ctx := context.Background()

	c, err := credentials.NewIamCredentialsClient(ctx)
	if err != nil {
		log.Fatalf("%v", err)
	}
	defer c.Close()

	idreq := &credentialspb.GenerateIdTokenRequest{
		Name:         fmt.Sprintf("projects/-/serviceAccounts/%s", serviceAccount),
		Audience:     aud,
		IncludeEmail: true,
	}
	idresp, err := c.GenerateIdToken(ctx, idreq)
	if err != nil {
		log.Fatalf("%v", err)
	}

	log.Printf("IdToken %v", idresp.Token)

	req, err := http.NewRequest(http.MethodGet, url, nil)
	if err != nil {
		log.Fatalf("%v", err)
	}
	req.Header.Add("Authorization", "Bearer "+idresp.Token)

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		log.Println("Error on response.\n[ERROR] -", err)
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		log.Println("Error while reading the response bytes:", err)
	}
	log.Println(string([]byte(body)))

}
