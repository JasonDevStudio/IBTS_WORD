using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;
using Microsoft.Vbe.Interop.Forms;
using Newtonsoft.Json;
using System.Xml;
using System.Text.RegularExpressions; 

namespace IbtsWord
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private XmlDocument dataDoc;
        
        private DocDefinition definitionDTO;

        private void button1_Click(object sender, EventArgs e)
        {
            string xmlDefinitionPath = @"E:\ibts.definition.xml"; 
            string dataFilePath = @"E:\data.json";

            dataDoc = getDocData(dataFilePath);
            definitionDTO = getDocDefinition(xmlDefinitionPath);

            XmlNode node = dataDoc.SelectSingleNode("/data/pagingDTO.check2");
            XmlNode node2 = dataDoc.SelectSingleNode("/data/companyDTO/companyCode");

            // return;

            object path; //文件路径变量 
            object destPath; //文件路径变量 
            MSWord.Application wordApp = null; //Word 应用程序变量 
            MSWord.Document wordDoc = null; //Word文档变量 

             path = @"E:\T22.docm"; //路径 
             destPath = @"E:\T25.docx"; //路径 

             //由于使用的是COM库，因此有许多变量需要用Missing.Value代替 
             Object Nothing = Missing.Value;

             try
             {
                 wordApp = new MSWord.ApplicationClass(); //初始化 


                 wordDoc = wordApp.Documents.Open(ref path,
                    ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                    ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                    ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);

                 foreach (MSWord.InlineShape ishape in wordDoc.InlineShapes)
                 {
                     MSWord.Field f = ishape.Field;

                     if (ishape.Type != MSWord.WdInlineShapeType.wdInlineShapeOLEControlObject)
                         continue;

                     String controlName = ishape.OLEFormat.Object.GetType().InvokeMember("Name", System.Reflection.BindingFlags.GetProperty, null, ishape.OLEFormat.Object, null).ToString();
                     DocItemDefinitionDTO item = definitionDTO.getItem(controlName);
                     if (item == null)
                         continue;

                     // 判断为 CheckBox
                     if (f.Code.Text.Contains("Forms.CheckBox.1"))
                     {
                         Microsoft.Vbe.Interop.Forms.CheckBox control = (Microsoft.Vbe.Interop.Forms.CheckBox)ishape.OLEFormat.Object;
                         setCheckBoxControl(item, control);
                     } if (f.Code.Text.Contains("Forms.Label.1"))
                     {
                         Microsoft.Vbe.Interop.Forms.Label control = (Microsoft.Vbe.Interop.Forms.Label)ishape.OLEFormat.Object;
                         setLabelControl(item, control);
                     }
                     else
                         continue;
                 }

                 MSWord.ContentControls contentControls = wordDoc.ContentControls;
                 foreach (MSWord.ContentControl control in contentControls)
                 {
                     String controlName = control.Tag;

                     DocItemDefinitionDTO item = definitionDTO.getItem(controlName);
                     if (item == null)
                         continue;

                     if (control.Type == MSWord.WdContentControlType.wdContentControlRichText)
                     {
                         setRichTextContentBoxControl(item, control);
                     }
                 }

                 //WdSaveFormat 为Word 文档的保存格式 
                 object format = MSWord.WdSaveFormat.wdFormatDocumentDefault;

                 //将wordDoc文档对象的内容保存为DOC文档 
                 wordDoc.SaveAs(ref destPath, ref format, ref Nothing, ref Nothing,
                        ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                        ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                        ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                 MessageBox.Show("Success!");
             }
             catch (Exception e1)
             {
                 MessageBox.Show("Error:" + e1.Message);
             }
             finally
             {
                 //关闭wordDoc文档对象 
                 if (wordDoc != null) wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);

                 //关闭wordApp组件对象
                 if (wordApp != null) wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
             }
 
        }

        private int setRichTextContentBoxControl(DocItemDefinitionDTO item, MSWord.ContentControl control)
        {
            control.Range.Font.Underline = MSWord.WdUnderline.wdUnderlineSingle;
            string a1 = "Trans Curr: $taskNo$   " + "RMB" + "      Settle Curr:" + "HKD" + "         ACCR               " + "6.5" + "  %";
            string a2 = a1 + "\r\a" + a1 + "\r\a" + a1;

            if ("RichText".Equals(item.ControlType))
            {
                XmlNode node = dataDoc.SelectSingleNode(item.DataPath);
                if (node != null)
                {
                    control.Range.FormattedText.Text = node.InnerText;
                    return 1;
                }
            } else if ("ListItem".Equals(item.ControlType))
            {
                string template = control.Range.Text;

                Regex myRegex = new Regex("\\$\\w+\\$", RegexOptions.IgnoreCase);
                if (myRegex.IsMatch(template))
                {
                    MatchCollection myMatch = myRegex.Matches(template);
                    XmlNodeList nodes = dataDoc.SelectNodes(item.DataPath);
                    if (nodes != null && nodes.Count > 0)
                    {
                        string[] resultStrs = new string[nodes.Count];
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            resultStrs[i] = template;
                            XmlNode node = nodes[i];
                            if (node != null)
                            {
                                foreach (Match match in myMatch)
                                {
                                    string itemName = match.Value;
                                    DocItemDefinitionDTO item2 = definitionDTO.getItem(item, itemName.Substring(1, itemName.Length - 2));
                                    if (item2 != null)
                                    {
                                        XmlNode nodeChild = node.SelectSingleNode(item2.DataPath);
                                        if (nodeChild != null)
                                        {
                                            resultStrs[i] = resultStrs[i].Replace(itemName, nodeChild.InnerText);
                                        }
                                    }
                                }
                            }
                        }
                        string resultStr = "";
                        foreach (string result in resultStrs)
                        {
                            resultStr += result + "\r\a";
                        }

                        control.Range.Text = resultStr;
                    }
                }
            }
            return 0;
        }

        private int setCheckBoxControl(DocItemDefinitionDTO item, Microsoft.Vbe.Interop.Forms.CheckBox control)
        {
            if ("CheckBox".Equals(item.ControlType))
            {
                XmlNode node = dataDoc.SelectSingleNode(item.DataPath);
                if (node != null)
                {
                    if ("true".Equals(node.InnerText,StringComparison.CurrentCultureIgnoreCase))
                    {
                        control.set_Value(true);
                    }
                    else
                    {
                        control.set_Value(false);
                    }
                    return 1;
                }
            }
            return 0;
        }

        private int setLabelControl(DocItemDefinitionDTO item, Microsoft.Vbe.Interop.Forms.Label control)
        {
            if ("Label".Equals(item.ControlType))
            {
                XmlNode node = dataDoc.SelectSingleNode(item.DataPath);
                if (node != null)
                {
                    control.Caption = node.InnerText;
                    return 1;
                }
            } 
            return 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private XmlDocument getDocData(string dataFilePath)
        {
            FileInfo myFile = new FileInfo(dataFilePath);  
            StreamReader sr = myFile.OpenText();
            string json = sr.ReadToEnd();
            sr.Close();
            
            //将JSON字符串转换为XmlDocument对象
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
            return doc;
        }

        private DocDefinition getDocDefinition(string definitionFilePath)
        {
            DocDefinition definitionDTO = new DocDefinition();

            //初始化一个xml实例
            XmlDocument xml = new XmlDocument();

            //导入指定xml文件
            xml.Load(definitionFilePath);

            XmlNode root = xml.SelectSingleNode("/root");
            if (root.HasChildNodes)
            {
                XmlNodeList childlist = root.ChildNodes;
                foreach (XmlNode node in childlist)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        XmlElement element = (XmlElement)node;
                        definitionDTO.addElement(element);
                   }
                }
            }

            return definitionDTO;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PostMessage post = new PostMessage();
            //richTextBox1.Text = post.login();

            //richTextBox1.Text = post.getTaskList();

            string json = "{\"SERVICE_RESPONSE_CODE\":\"S0005\",\"success\":true,\"pagingDTO\":{\"start\":0,\"limit\":10,\"condition\":{\"dto\":{\"userName\":null,\"agentId\":null,\"agentDTO\":null,\"userId\":null,\"userType\":null,\"loginId\":null,\"userEmail\":null,\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0}},\"totalRecord\":44,\"totalPages\":5,\"resultList\":[{\"userName\":\"xiaoxiao\",\"agentId\":\"20001\",\"agentDTO\":{\"address\":null,\"order\":\"20001\",\"agentName\":\"Comconn Inc.\",\"agentCode\":\"30130840\",\"agentId\":\"20001\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":\"Comconn\",\"city\":null,\"agentPro\":\"2\",\"branchId\":\"105\",\"agentGrade\":\"1\",\"nationName\":\"United States\",\"nationCode\":\"0840\",\"testContractor\":\"Sherry Wu\",\"email\":\"sherrywu@comconnpay.com\",\"zipCode\":null,\"tel\":\"021-60823337-318\",\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":\"Comconn Inc.\",\"chShotName\":\"Comconn\",\"agentTypeDesc\":null},\"userId\":\"1402543145180\",\"userType\":\"1\",\"loginId\":\"10001\",\"userEmail\":\"1@qq.com\",\"userTel\":\"\",\"userRemark\":\"\",\"userFlag\":\"1\",\"userOrder\":999},{\"userName\":\"test001\",\"agentId\":\"20001\",\"agentDTO\":{\"address\":null,\"order\":\"20001\",\"agentName\":\"Comconn Inc.\",\"agentCode\":\"30130840\",\"agentId\":\"20001\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":\"Comconn\",\"city\":null,\"agentPro\":\"2\",\"branchId\":\"105\",\"agentGrade\":\"1\",\"nationName\":\"United States\",\"nationCode\":\"0840\",\"testContractor\":\"Sherry Wu\",\"email\":\"sherrywu@comconnpay.com\",\"zipCode\":null,\"tel\":\"021-60823337-318\",\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":\"Comconn Inc.\",\"chShotName\":\"Comconn\",\"agentTypeDesc\":null},\"userId\":\"30001\",\"userType\":\"1\",\"loginId\":\"test001\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test002\",\"agentId\":\"20002\",\"agentDTO\":{\"address\":null,\"order\":\"20002\",\"agentName\":\"Spitamen Bank\",\"agentCode\":\"30030762\",\"agentId\":\"20002\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"102\",\"agentGrade\":\"1\",\"nationName\":\"Tajikistan\",\"nationCode\":\"0762\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30002\",\"userType\":\"1\",\"loginId\":\"test002\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test003\",\"agentId\":\"20003\",\"agentDTO\":{\"address\":null,\"order\":\"20003\",\"agentName\":\"Russlavbank\",\"agentCode\":\"29280643\",\"agentId\":\"20003\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"102\",\"agentGrade\":\"1\",\"nationName\":\"Russian Federation\",\"nationCode\":\"0643\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30003\",\"userType\":\"1\",\"loginId\":\"test003\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test004\",\"agentId\":\"20004\",\"agentDTO\":{\"address\":null,\"order\":\"20004\",\"agentName\":\"PaySquare (via Omnipay)\",\"agentCode\":\"29510528\",\"agentId\":\"20004\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"107\",\"agentGrade\":\"1\",\"nationName\":\"Netherlands\",\"nationCode\":\"0528\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30004\",\"userType\":\"1\",\"loginId\":\"test004\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test005\",\"agentId\":\"20005\",\"agentDTO\":{\"address\":null,\"order\":\"20005\",\"agentName\":\"PaySquare (via Omnipay) Belgium \",\"agentCode\":\"29510056\",\"agentId\":\"20005\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"107\",\"agentGrade\":\"2\",\"nationName\":\"Belgium\",\"nationCode\":\"0056\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":{\"address\":null,\"order\":\"20004\",\"agentName\":\"PaySquare (via Omnipay)\",\"agentCode\":\"29510528\",\"agentId\":\"20004\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"107\",\"agentGrade\":\"1\",\"nationName\":\"Netherlands\",\"nationCode\":\"0528\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30005\",\"userType\":\"1\",\"loginId\":\"test005\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test006\",\"agentId\":\"20006\",\"agentDTO\":{\"address\":null,\"order\":\"20006\",\"agentName\":\"PaySquare (via Omnipay) Luxembourg \",\"agentCode\":\"29510442\",\"agentId\":\"20006\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"107\",\"agentGrade\":\"2\",\"nationName\":\"Luxembourg\",\"nationCode\":\"0442\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":{\"address\":null,\"order\":\"20004\",\"agentName\":\"PaySquare (via Omnipay)\",\"agentCode\":\"29510528\",\"agentId\":\"20004\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"107\",\"agentGrade\":\"1\",\"nationName\":\"Netherlands\",\"nationCode\":\"0528\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30006\",\"userType\":\"1\",\"loginId\":\"test006\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test007\",\"agentId\":\"20007\",\"agentDTO\":{\"address\":null,\"order\":\"20007\",\"agentName\":\"Spitamen Bank Hongkong\",\"agentCode\":\"30030344\",\"agentId\":\"20007\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"111\",\"agentGrade\":\"2\",\"nationName\":\"Hong Kong\",\"nationCode\":\"0344\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":{\"address\":null,\"order\":\"20002\",\"agentName\":\"Spitamen Bank\",\"agentCode\":\"30030762\",\"agentId\":\"20002\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"102\",\"agentGrade\":\"1\",\"nationName\":\"Tajikistan\",\"nationCode\":\"0762\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30007\",\"userType\":\"1\",\"loginId\":\"test007\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test008\",\"agentId\":\"20008\",\"agentDTO\":{\"address\":null,\"order\":\"20008\",\"agentName\":\"ITAU\",\"agentCode\":\"27540076\",\"agentId\":\"20008\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"105\",\"agentGrade\":\"1\",\"nationName\":\"Brazil\",\"nationCode\":\"0076\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30008\",\"userType\":\"1\",\"loginId\":\"test008\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0},{\"userName\":\"test009\",\"agentId\":\"20009\",\"agentDTO\":{\"address\":null,\"order\":\"20009\",\"agentName\":\"Closed Joint Stock Company Kyrgyz Investment and Credit Bank\",\"agentCode\":\"27730417\",\"agentId\":\"20009\",\"flag\":\"1\",\"agentType\":\"1\",\"enAgentShotName\":null,\"city\":null,\"agentPro\":\"2\",\"branchId\":\"102\",\"agentGrade\":\"1\",\"nationName\":\"Kyrgyzstan\",\"nationCode\":\"0417\",\"testContractor\":null,\"email\":null,\"zipCode\":null,\"tel\":null,\"fax\":null,\"helpdesk\":null,\"parentAgent\":null,\"chAgentName\":null,\"chShotName\":null,\"agentTypeDesc\":null},\"userId\":\"30009\",\"userType\":\"1\",\"loginId\":\"test009\",\"userEmail\":\"lijiansong@purple-river.com\",\"userTel\":null,\"userRemark\":null,\"userFlag\":null,\"userOrder\":0}]}}";
            Object obj = JsonUtil.getObject(json);
        }

        // 参考代码
        // 使用 ContentControl
        // foreach(MSWord.ContentControl t in s){
        //    if (t.Type == MSWord.WdContentControlType.wdContentControlRichText)
        //    {
        //        //RichTextContentControl t1 = t as RichTextContentControl;
        //        //t1.Text = "text name ";
        //        t.Range.Text = "text name";
        //    }
        //    t.Title = "Please enter your name"; 
        //}
 

        //using System.Xml;
        ////初始化一个xml实例
        //XmlDocument xml=new XmlDocument();
        ////导入指定xml文件
        //xml.Load(path);
        //xml.Load(HttpContext.Current.Server.MapPath("~/file/bookstore.xml"));
        ////指定一个节点
        //XmlNode root=xml.SelectSingleNode("/root");
        ////获取节点下所有直接子节点
        //XmlNodeList childlist=root.ChildNodes;
        ////判断该节点下是否有子节点
        //root.HasChildNodes;
        ////获取同名同级节点集合
        //XmlNodeList nodelist=xml.SelectNodes("/Root/News");
        ////生成一个新节点
        //XmlElement node=xml.CreateElement("News");
        ////将节点加到指定节点下，作为其子节点
        //root.AppendChild(node);
        ////将节点加到指定节点下某个子节点前
        //root.InsertBefore(node,root.ChildeNodes[i]);
        ////为指定节点的新建属性并赋值
        //node.SetAttribute("id","11111");
        ////为指定节点添加子节点
        //root.AppendChild(node);
        ////获取指定节点的指定属性值
        //node["id"].Value;
        ////获取指定节点中的文本
        //string content=node.InnerText;
        ////保存XML文件
        //string path=Server.MapPath("~/file/bookstore.xml");
        //xml.Save(path);

        //将XmlDocument对象转换为JSON字符串
        //XmlDocument xdoc = new XmlDocument(); 
        //xdoc.LoadXml(xml);  
        //string jsonText = JsonConvert.SerializeXmlNode(xdoc); 

        //将JSON字符串转换为XmlDocument对象
        //XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);

        // 插入checkbox选择框符号
        //object fontname = "Wingdings 2";
        //object uic = true;
        //control.Range.InsertSymbol(-4014, ref fontname, ref uic, ref Nothing);

    }

    public class DocDefinition
    {
        public DocDefinition(){
            itemList = new List<DocItemDefinitionDTO>(); 
        }

        public List<DocItemDefinitionDTO> itemList;

        public void addElement(XmlElement element)
        {
            DocItemDefinitionDTO dto = new DocItemDefinitionDTO();
            addElement(dto, element);
            itemList.Add(dto);
        }

        public void addElement(DocItemDefinitionDTO dto, XmlElement element)
        {
            dto.Name = element.GetAttribute("name");
            dto.ControlName = element.GetAttribute("controlName");
            dto.ControlType = element.GetAttribute("controlType");
            dto.DataPath = element.GetAttribute("dataPath");

            foreach (XmlElement node in element.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    XmlElement ele = (XmlElement)node;
                    DocItemDefinitionDTO childDto = new DocItemDefinitionDTO();
                    addElement(childDto, ele);
                    dto.child.Add(childDto);
                }
            }
        }

        public DocItemDefinitionDTO getItem(string controlName)
        {
            foreach (DocItemDefinitionDTO item in itemList)
            {
                if (item.ControlName == controlName)
                {
                    return item;
                }
            }
            return null;
        }

        public DocItemDefinitionDTO getItem(DocItemDefinitionDTO dto, string controlName)
        {
            if (dto!=null && dto.child!=null && dto.child.Count>0)
            {
                foreach (DocItemDefinitionDTO item in dto.child)
                {
                    if (item.ControlName == controlName)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

    }

    public class DocItemDefinitionDTO
    {
        public DocItemDefinitionDTO()
        {
            child = new List<DocItemDefinitionDTO>();
        }
        public string Name  { get; set; }
        public string ControlName  { get; set; }
        public string ControlType { get; set; }
        public string DataPath { get; set; }
        public List<DocItemDefinitionDTO> child { get; set; }
    }

 }
