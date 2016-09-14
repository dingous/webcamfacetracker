using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Upload : System.Web.UI.Page
{

	public class PostData
	{
		public string Base64Data { get; set; }
		public int BarcodeType { get; set; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		var fImg = AppDomain.CurrentDomain.BaseDirectory + @"\demo\filepicker2\uploader\files\avatar1.jpg";
		var urlImg = GetDominioUrl() + "/demo/filepicker2/uploader/files/avatar1.jpg";

		foreach (string fileName in Request.Files)
		{
			HttpPostedFile file = Request.Files[fileName];

			file.SaveAs(fImg);


			var fileLength = new FileInfo(fImg).Length;

			using (var img = System.Drawing.Image.FromFile(fImg))
			{
				var height = img.Height;
				var width = img.Width;


				var obj = new Object[1];

				obj[0] = new
				{
					name = "avatar1.jpg",
					url = urlImg,
					size = fileLength,
					time = 0,
					type = "image/jpeg",
					extension = "jpg",
					width = width,
					height = height,
					versions = new
					{
						avatar = new
						{
							name = "avatar1.jpg",
							url = urlImg,
							size = 21124,
							width = 300,
							height = 300
						}
					}
				};

				Response.ContentType = "application/json";
				Response.Write(JsonConvert.SerializeObject(obj));
				Response.End();

			}


		}


		if (Request.Form["x"] != null)
		{
			var fileOld = new FileInfo(fImg);

			decimal width = Convert.ToDecimal(Request.Form["width"].Replace(".", ","));
			decimal height = Convert.ToDecimal(Request.Form["height"].Replace(".", ","));
			decimal sacaleX = Convert.ToDecimal(Request.Form["scaleX"].Replace(".", ","));
			decimal sacaleY = Convert.ToDecimal(Request.Form["scaleY"].Replace(".", ","));
			double x = Convert.ToDouble(Request.Form["x"].Replace(".", ","));
			double y = Convert.ToDouble(Request.Form["y"].Replace(".", ","));


			var img = System.Drawing.Image.FromFile(fImg);
			var cropped = CropImage(img, Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(width), Convert.ToInt32(height));
			img.Dispose();

			cropped.Save(fImg);			

			var obj = new Object();
			obj = new
			{
				name = "avatar1.jpg",
				url = urlImg,
				size = fileOld.Length,
				time = 0,
				type = "image/jpeg",
				extension = "jpg",
				width = width,
				height = height,
				versions = new
				{
					avatar = new
					{
						name = "avatar1.jpg",
						url = urlImg,
						size = fileOld.Length,
						width = 300,
						height = 300
					}
				}
			};

			Response.ContentType = "application/json";
			Response.Write(JsonConvert.SerializeObject(obj));
			Response.End();

		}

	}

	public static string GetDominioUrl()
	{
		string dirVirtual = System.Web.HttpContext.Current.Request.ApplicationPath;
		string virtualPath = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"].ToString() + dirVirtual;		
		virtualPath = "http://" + virtualPath;
		return virtualPath;
	}

	public static Bitmap CropImage(System.Drawing.Image source, int x, int y, int width, int height)
	{
		Rectangle crop = new Rectangle(x, y, width, height);

		var bmp = new Bitmap(crop.Width, crop.Height);
		using (var gr = Graphics.FromImage(bmp))
		{
			gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
		}
		return bmp;
	}
}