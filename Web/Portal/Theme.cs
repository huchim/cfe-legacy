using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfe.Web.Portal
{
    public class Theme
    {
        private string serviceUrl = "http://10.7.9.208:81/webservices/WSCFEMenu/Service.asmx";
        private Designer designerObj;

        private ThemeSection htmlData;

        /// <summary>
        /// Se conecta al servidor de diseño válido por medio de un servicio web.
        /// </summary>
        public Theme()
        {
            // Verificar que existe la configuración de la ruta al servicio.
            this.serviceUrl = (System.Configuration.ConfigurationManager.AppSettings["Cfe.Web.Portal.Theme.ServiceUrl"] ?? this.serviceUrl).ToString();

            this.InitializeDesigner();
        }

        /// <summary>
        /// Se conecta al servidor de diseño válido por medio de un servicio web especificado.
        /// </summary>
        /// <param name="serviceUrl">Dirección Url al servicio web.</param>
        public Theme(string serviceUrl)
        {
            this.serviceUrl = serviceUrl;
            this.InitializeDesigner();
        }

        private void InitializeDesigner()
        {
            // Inicializo el diseñador.
            this.designerObj = new Designer(this.serviceUrl);
        }


        public void Update()
        {
            this.HtmlSections = new ThemeSection();


            this.Name = (System.Configuration.ConfigurationManager.AppSettings["Cfe.Web.Portal.Theme.Name"] ?? "Default").ToString();
            this.Section = (System.Configuration.ConfigurationManager.AppSettings["Cfe.Web.Portal.Theme.Section"] ?? "Default").ToString();

            this.htmlData = new ThemeSection();

            this.htmlData.Header = this.designerObj.Header(this.Name, this.Section);

            this.htmlData.Footer = this.designerObj.Footer();


        }

        public ThemeSection HtmlSections
        {
            get
            {
                if (this.htmlData == null)
                {
                    this.Update();
                }

                return this.htmlData;

            }
            set { this.htmlData = value; }
        }

        /// <summary>
        /// Nombre de la aplicación.
        /// </summary>
        public string Name { get; set; }

        public string Section { get; set; }
    }
}
