#if IAP
using LitJson;

using System;
using System.Collections.Generic;
using UnityEngine;
using ZGame.SDK;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using ZGame;

namespace ZGame.SDK.IAP
{
    public class PayVerifyData
    {
        public string inner_order_id;
        public string inner_app_id;

        public int pay_platform;
        public string receipt_data;


    }

    public class AppleStorePayVerifyData : PayVerifyData
    {

        public AppleStorePayVerifyData(string inner_order_id, string inner_app_id, string receipt_data, int pay_platform)
        {
            this.inner_order_id = inner_order_id;


            this.receipt_data = receipt_data;
            this.inner_app_id = inner_app_id;
            this.pay_platform = pay_platform;
        }
    }

    public class GooglePlayPayVerifyData : PayVerifyData
    {


        public GooglePlayPayVerifyData(string inner_order_id, string inner_app_id, string receipt_data, int pay_platform)
        {
            this.inner_order_id = inner_order_id;
            this.receipt_data = receipt_data;
            this.inner_app_id = inner_app_id;
            this.pay_platform = pay_platform;
        }
    }



    public class PayVerify : Singleton<PayVerify>
    {
        Dictionary<string, PayVerifyData> PayVerifyDataDic = new Dictionary<string, PayVerifyData>();




        public void AddPayVerifyData(string inner_order_id, string inner_app_id, string receipt_data, int pay_platform)
        {
            if (PayVerifyDataDic.ContainsKey(inner_order_id))
            {
                Debug.LogError("duplicated inner_order_id:" + inner_order_id);
                return;
            }
            if (pay_platform == PaymentChannelId.GooglePlay)
            {
                PayVerifyDataDic.Add(inner_order_id, new GooglePlayPayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform));
            }
            else if (pay_platform == PaymentChannelId.AppleStore)
            {
                PayVerifyDataDic.Add(inner_order_id, new AppleStorePayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform));
            }
            else
            {
                Debug.LogError("unsupported pay_platform:" + pay_platform);
            }
        }

        public void DeleteVerifyData(string inner_order_id)
        {
            if (PayVerifyDataDic.ContainsKey(inner_order_id))
            {
                Debug.LogError("delete local order data success ，inner_order_id：" + inner_order_id);
                PayVerifyDataDic.Remove(inner_order_id);
            }
        }

        public void WritePayVerifyDataToLocal()
        {
            JsonData rootObj = new JsonData();

            foreach (var item in PayVerifyDataDic)
            {

                var newJson = new JsonData();
                newJson["inner_order_id"] = item.Key;

                if (ZGame.Config.paymentChannelId == PaymentChannelId.AppleStore)
                {
                    var data = item.Value as AppleStorePayVerifyData;
                    newJson["inner_app_id"] = data.inner_app_id;
                    newJson["receipt_data"] = data.receipt_data;
                    newJson["pay_platform"] = data.pay_platform;
                }
                else if (ZGame.Config.paymentChannelId == PaymentChannelId.GooglePlay)
                {
                    var data = item.Value as GooglePlayPayVerifyData;
                    newJson["inner_app_id"] = data.inner_app_id;
                    newJson["receipt_data"] = data.receipt_data;
                    newJson["pay_platform"] = data.pay_platform;
                }


                rootObj.Add(newJson);
            }

            string jsonStr = rootObj.ToJson();
            if (jsonStr == "null")
            {
                jsonStr = "[]";
            }
            IOTools.WriteStringToUpdateDir("pay_verify_data.txt", jsonStr);
        }

        public PayVerifyData ReadFirstVerifyDataFromLocal()
        {
            if (PayVerifyDataDic.Count > 0)
            {
                foreach (var item in PayVerifyDataDic)
                {
                    return item.Value;
                }
            }
            return null;
        }
        public void ReadPayVerifyDataFromLocal()
        {
            PayVerifyDataDic.Clear();
            var str = IOTools.ReadStringFromUpdateDir("pay_verify_data.txt");
            if (str == "")
            {
                return;
            }

            JsonData obj = JsonMapper.ToObject(str);
            if (obj.Count > 0)
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    var item = obj[i];
                    if (Config.paymentChannelId == PaymentChannelId.AppleStore)
                    {
                        string inner_order_id = item["inner_order_id"].ToString();
                        string inner_app_id = item["inner_app_id"].ToString();
                        string receipt_data = item["receipt_data"].ToString();
                        int pay_platform = int.Parse(item["pay_platform"].ToString());

                        AddPayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform);
                    }
                    else if (Config.paymentChannelId == PaymentChannelId.GooglePlay)
                    {

                        string inner_order_id = item["inner_order_id"].ToString();
                        string inner_app_id = item["inner_app_id"].ToString();
                        string receipt_data = item["receipt_data"].ToString();
                        int pay_platform = int.Parse(item["pay_platform"].ToString());

                        AddPayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform);

                    }
                }
            }
        }


    }




    //官方文档
    //https://docs.unity3d.com/Manual/UnityIAPSettingUp.html
    public class IAPMgr : Singleton<IAPMgr>, IStoreListener
    {
        private IStoreController controller;
        private IAppleExtensions appleExtensions;
        private IGooglePlayStoreExtensions googlePlayStoreExtensions;
        private ConfigurationBuilder builder;

        //使用这个解析IAP成功后的receipt
        private UnityEngine.Purchasing.Security.CrossPlatformValidator validator;

        public void Init(string[] consumableProductIds, string[] nonConsumableProductIds, string[] subscriptionProductIds)
        {
            Debug.Log("begin init IAP");
            var module = StandardPurchasingModule.Instance();
            builder = ConfigurationBuilder.Instance(module);

            this.addConsumableProducts(consumableProductIds);
#if !UNITY_EDITOR
            validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            UnityPurchasing.Initialize(this, builder); 
#endif
        }


        void addConsumableProducts(string[] productIds)
        {
            if (productIds == null)
            {
                return;
            }
            int count = productIds.Length;
            Debug.LogError("add consumable products,count:" + count);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Debug.LogError("add:" + productIds[i]);
                    builder.AddProduct(productIds[i], ProductType.Consumable);
                }
            }

        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAP initialize success");
            this.controller = controller;

            this.appleExtensions = extensions.GetExtension<IAppleExtensions>();
            this.googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("IAP InitializeFailed, reason:" + error.ToString());
        }
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("IAP InitializeFailed, reason:" + error.ToString() + ",message:" + message);
        }
        public void OnPurchaseFailed(Product i, PurchaseFailureReason r)
        {
            Debug.LogError("OnPurchaseFailed,reason:" + r.ToString());
            if (this.onPurchaseFailed != null)
            {
                this.onPurchaseFailed(r);
                this.onPurchaseFailed = null;
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.LogError("purchase finished ");
            //Debug.LogE("receipt:" + e.purchasedProduct.receipt);
            var result = validator.Validate(e.purchasedProduct.receipt);
            foreach (IPurchaseReceipt productReceipt in result)
            {
                //Debug.Log(" --------->productId:" + productReceipt.productID);
                //Debug.Log(" --------->purchaseData:" + productReceipt.purchaseDate);
                //Debug.Log(" --------->transactionID:" + productReceipt.transactionID);
#if UNITY_ANDROID
                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                if (google != null)
                {

                    //Debug.Log(" --------->purchaseState:" + google.purchaseState);
                    //Debug.Log(" --------->purchaseToken:" + google.purchaseToken);
                    //Debug.Log(" --------->packageName:" + google.packageName);
                    if (this.onPurchaseSuccess != null)
                    {
                        //json格式字符串
                        string receiptStr = e.purchasedProduct.receipt;
                        this.onPurchaseSuccess(receiptStr);
                        this.onPurchaseSuccess = null;
                    }
                }

#elif UNITY_IOS
            AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
            if (apple != null)
            {
                if (this.onPurchaseSuccess != null)
                {
                    
                    this.onPurchaseSuccess(e.purchasedProduct.receipt );
                    this.onPurchaseSuccess = null;
                }
            }
#endif
            }

            return PurchaseProcessingResult.Complete;
        }



        private Action<PurchaseFailureReason> onPurchaseFailed;
        private Action<string> onPurchaseSuccess;
        public void PurchaseProduct(string productId, Action onControllerIsNull, Action<PurchaseFailureReason> onFailed, Action<string> onSuccess)
        {
            Debug.LogError("purchaseProduct, productId:" + productId);
            this.onPurchaseFailed = onFailed;
            this.onPurchaseSuccess = onSuccess;

            if (controller != null)
            {
                var product = controller.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    controller.InitiatePurchase(productId);
                }
                else
                {
                    Debug.LogError("no product with productId:" + productId);
                    if (this.onPurchaseFailed != null)
                    {
                        this.onPurchaseFailed(PurchaseFailureReason.ProductUnavailable);// "No product with id:" + productId
                    }
                }
            }
            else
            {
                //controller 为null，表明IAP初始化就已经失败，通常是由于GooglePlayService不可用等原因导致
                Debug.LogError("controller is null,can not do purchase!!");
                if (onControllerIsNull != null)
                {
                    onControllerIsNull();
                }
            }
        }


    }
}
#endif