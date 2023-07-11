using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TursimoReal.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TursimoReal.Controllers
{
    public class DocumnetsController : Controller
    {
        // GET: Documnets
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerarPDF()
        {
            Document doc = new Document();
            
            MemoryStream memoryStream = new MemoryStream();

            PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

            doc.Open();

            //Encabazado
            string NDocumento = "Numero de Documento: " /*Numero del documento*/;
            Paragraph header = new Paragraph("Detalles de Arriendo" + "\n" + NDocumento);

            header.Alignment = Element.ALIGN_CENTER;
            doc.Add(header);

            //Informacio del Arriendo
            
            string PrimeraParte = "En este documento se le entregara la informacion, de que el usuario: " + /*Dato Usuario*/ "\n" + /*Nombre - Apellido - Correo - Numero de contacto*/
                " Realizo el arriendo del departamento: "+ /*Nombre Departamento*/ "" + " Ubicado en la region: " + /*Nombre Region*/ "\n" +
                "Direccion: " +/*Direccion*/ "";
            string SegundoParte = "Fecha de inicio" + /*Fecha de inicio de arriendo*/"" + "Fecha de termino: " + /*Fecha de termino de arriendo*/"";
            string TerceraParte = "Cantidad Acompañantes: " + /*Ingresar cantidad de acompañantes*/ "" + "Servicios Solicitados: " +/*Servicios*/"";
            string CuartaParte = "Numero de habitacion: " + /*Numero de la habitacion*/ "" + "Tiempo de estancia: " + /*Cantidad de dias que se quedara el usuario*/"";
            string QuintaParte = "Tipo de devolucion: " + /*Devolucion*/ "" + "Tipo de pago" + /*Metodo de pago*/"" + "Valor Arriendo: " + /*Valor del arriendo*/"\n"+
                "Valor servicios: " /**/ + "Total: " /*Valor total del arriendo*/ + "Firma: " /*Firma del usuario*/;

            doc.Add(new Paragraph(PrimeraParte));
            doc.Add(new Paragraph(SegundoParte));
            doc.Add(new Paragraph(TerceraParte));
            doc.Add(new Paragraph(CuartaParte));
            doc.Add(new Paragraph(QuintaParte));

            byte[] fileBytes = memoryStream.ToArray();
            memoryStream.Close();
            return File(fileBytes, "application/pdf", "tabla.pdf");
        }
    }
}