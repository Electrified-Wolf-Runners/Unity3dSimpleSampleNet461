using ABI.Contracts.test.ContractDefinition;
using Nethereum.JsonRpc.UnityClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartContract : MonoBehaviour
{
    private string url = "https://ropsten.infura.io/v3/bd5e6947e479474580a36dc8da4183c5";
    private string account = "0x15B700bb0BFf2460DBeB905EEBCAa6f94471178A";
    private string contractAddress = "0x1556036a587e51eDc0D7487eA981AE820D6E565a";

    private string privateKey = "65f9cbb3aefa8a632c939807c567291938d4035f5b8601558e4ba2d3cc840230";

    IEnumerator Start()
    {
        yield return StartCoroutine(Mint());
    }

    public IEnumerator Balance()
    {
        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfOutputDTO>(url, account);
        yield return queryRequest.Query(new BalanceOfFunction() { Account = account }, contractAddress);

        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        Debug.Log(dtoResult.ReturnValue1);

    }

    public IEnumerator Mint()
    {
        var transactionTransferRequest = new TransactionSignedUnityRequest(url, privateKey);
        transactionTransferRequest.UseLegacyAsDefault = true;
        var transactionMessage = new MintFunction();

        yield return transactionTransferRequest.SignAndSendTransaction(transactionMessage, contractAddress);
        var transactionTransferHash = transactionTransferRequest.Result;

        Debug.Log("Transfer txn hash:" + transactionTransferHash);

        TransactionReceiptPollingRequest transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
        yield return transactionReceiptPolling.PollForReceipt(transactionTransferHash, 2);
        var transferReceipt = transactionReceiptPolling.Result;

        Debug.Log("Minted");

    }
}
