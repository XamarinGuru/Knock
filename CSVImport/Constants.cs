using System;
using System.Collections.Generic;
using System.Text;

namespace knock
{
    public class Constants
    {
        #region credentials
       
        public const string AWSiOSDebugARN = "arn:aws:sns:eu-west-1:240452127123:app/APNS_SANDBOX/knockiOSDevelop-new";//"arn:aws:sns:eu-west-1:756616156915:app/APNS_SANDBOX/knockiOSDevelop";
        public const string AWSiOSReleaseARN = "arn:aws:sns:eu-west-1:240452127123:app/APNS/knockiOSProd-new";//"arn:aws:sns:eu-west-1:756616156915:app/APNS/knockiOSProd";
        public const string AWSAndroidARN = "arn:aws:sns:eu-west-1:240452127123:app/GCM/knockAndroid-new";//"arn:aws:sns:eu-west-1:756616156915:app/GCM/knockDroid-new";
        public const string AWScognitoCredential = "eu-west-1:d0b3a0bf-18cb-4fa3-a1ef-d07a44c9770c";//"eu-west-1:b101c601-66f3-4c4b-9e58-4d1e2d56ec56";

		public const string ASWS3accesskey = "AKIAIDW6K3DQQLXQDXRQ";
		public const string ASWS3secret = "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";
		public static Amazon.RegionEndpoint ASWS3Region = Amazon.RegionEndpoint.EUWest1;
		public const string ASWS3Bucket = "knock-app-new";
		public static Amazon.S3.S3StorageClass AWSS3StorageClass = Amazon.S3.S3StorageClass.Standard;
		public const string AWSS3Endpoint = "https://"+ASWS3Bucket+".s3-eu-west-1.amazonaws.com/";

		#endregion

		public const string websiteEndpoint = "http://letknock.com/";
		public const string sitoAPI = "https://api.letknock.com/v1/";
        public const string sitoAPIv2 = "https://api.letknock.com/v2/";
        public const string restAPIv3 = "https://api.letknock.com/v3/";
		public const string restAPIv4 = "https://api.letknock.com/v4/";
		public const string gatewayV1 = "https://api.letknock.com/v4/gateway.php";

		public const string googlePlacesEndpoint = "https://maps.googleapis.com/maps/api/place/nearbysearch/";
		public const string googleStaticMapEndpoint = "https://maps.googleapis.com/maps/api/";
		public const string googleStaticMapKey = "AIzaSyDPAZLPSuWFUcteAGU0_tIIoRwn1_1OqDs";
		public const string facebookEndpoint = "https://graph.facebook.com/v2.7/";

		public const string serviceImg = restAPIv4 + "res/sharedService.png";

		public static string formatoData = "yyyy-MM-dd HH:mm:ss";

		public static readonly string[] facebookPermissions = new[] { "user_about_me","email","user_birthday","user_relationships" };
		public const int msecWaitUIFacebook = 1000;

		#region images
		#if __ANDROID__
		public const string LoadingImageBig = "SFShareInternalViolet.png";
		public const string LoadingImageSmall = "SFShareInternalViolet.png";
		public const string ErrorImageBig = "SFShareInternalBlack.png";
		public const string ErrorImageSmall = "SFShareInternalBlack.png";
#endif
#if __IOS__
		public const string LoadingImageBig = "SFShareInternalViolet";
		public const string LoadingImageSmall = "SFShareInternalViolet";
		public const string ErrorImageBig = "SFShareInternalBlack";
		public const string ErrorImageSmall = "SFShareInternalBlack";
#endif
		#endregion

	}
}
