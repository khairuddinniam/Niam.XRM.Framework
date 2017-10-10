namespace Niam.XRM.Framework.Plugin
{
    public enum SdkMessageProcessingStepStage
    {
        //InitialPreoperation_Forinternaluseonly = 5,
        Prevalidation = 10,
        //InternalPreoperationBeforeExternalPlugins_Forinternaluseonly = 15,
        Preoperation = 20,
        //InternalPreoperationAfterExternalPlugins_Forinternaluseonly = 25,
        //MainOperation_Forinternaluseonly = 30,
        //InternalPostoperationBeforeExternalPlugins_Forinternaluseonly = 35,
        Postoperation = 40,
        //InternalPostoperationAfterExternalPlugins_Forinternaluseonly = 45,
        //Postoperation_Deprecated = 50,
        //FinalPostoperation_Forinternaluseonly = 55,
    }
}
