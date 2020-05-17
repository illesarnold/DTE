using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class PeasyHelper
    {

        const string BusinessNameSpace = "[BusinessNameSpace]";
        const string DataLayerNameSpace = "[DataLayerNameSpace]";
        const string ProjectName = "[ProjectName]";
        const string SolutionName = "[SolutionName]";
        const string ModelName = "[ModelName]";
        const string ModelCode = "[ModelCode]";

        //Templates
        private static string PeasyBaseDomainTemplate;
        private static string PeasyBaseDataProxyTemplate;
        private static string PeasyBaseServiceTemplate;
        private static string PeasyBaseRepositoryTemplate;
        private static string PeasyDomainTemplate;
        private static string PeasyDataProxyTemplate;
        private static string PeasyServiceTemplate;
        private static string PeasyRepositoryTemplate;

        public PeasyHelper()
        {
            ReadTemplateFiles();
        }

        private static void ReadTemplateFiles()
        {
            PeasyBaseDomainTemplate = File.ReadAllText("Templates/PeasyBaseDomain.tpl");
            PeasyBaseDataProxyTemplate = File.ReadAllText("Templates/PeasyBaseDataProxy.tpl");
            PeasyBaseServiceTemplate = File.ReadAllText("Templates/PeasyBaseService.tpl");
            PeasyBaseRepositoryTemplate = File.ReadAllText("Templates/PeasyBaseRepository.tpl");
            PeasyDomainTemplate = File.ReadAllText("Templates/PeasyDomain.tpl");
            PeasyDataProxyTemplate = File.ReadAllText("Templates/PeasyDataProxy.tpl");
            PeasyServiceTemplate = File.ReadAllText("Templates/PeasyService.tpl");
            PeasyRepositoryTemplate = File.ReadAllText("Templates/PeasyRepository.tpl");
        }

        public string GetDomainTemplate(string nameSpace, string modelCode)
        {
            return PeasyDomainTemplate
                .Replace(BusinessNameSpace, nameSpace)
                .Replace(ModelCode, modelCode);
        }
        public string GetDataProxyTemplate(string nameSpace, string projectName, string modelName)
        {
            return PeasyDataProxyTemplate
                .Replace(BusinessNameSpace, nameSpace)
                .Replace(ProjectName, projectName)
                .Replace(ModelName, modelName);
        }
        public string GetServiceTemplate(string nameSpace, string modelName)
        {
            return PeasyServiceTemplate
                .Replace(BusinessNameSpace, nameSpace)
                .Replace(ModelName, modelName);
        }
        public string GetRepositoryTemplate(string dataLayerNameSpace, string businessNameSpace, string modelName)
        {
            return PeasyRepositoryTemplate
               .Replace(DataLayerNameSpace, dataLayerNameSpace)
               .Replace(BusinessNameSpace, businessNameSpace)
               .Replace(ModelName, modelName);
        }
        public string GetDomainBaseTemplate(string businessNameSpace)
        {
            return PeasyBaseDomainTemplate
               .Replace(BusinessNameSpace, businessNameSpace);
        }
        public string GetDataProxyBaseTemplate(string businessNameSpace, string projectName)
        {
            return PeasyBaseDataProxyTemplate
               .Replace(BusinessNameSpace, businessNameSpace)
               .Replace(ProjectName, projectName);
        }
        public string GetServiceBaseTemplate(string businessNameSpace, string projectName)
        {
            return PeasyBaseServiceTemplate
               .Replace(BusinessNameSpace, businessNameSpace)
               .Replace(SolutionName, projectName);
        }
        public string GetRepositoryBaseTemplate(string dataLayerNameSpace, string businessNameSpace, string projectName)
        {
            return PeasyBaseRepositoryTemplate
               .Replace(DataLayerNameSpace, dataLayerNameSpace)
               .Replace(BusinessNameSpace, businessNameSpace)
               .Replace(ProjectName, projectName);
        }
    }
}

