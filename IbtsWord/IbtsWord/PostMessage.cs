using System;
using System.Collections;
using System.IO;

namespace IbtsWord
{
    /// <summary> 
    /// Summary description for PostMessage. 
    /// </summary> 
    public class PostMessage
    {
        public PostMessage()
        {
        }

        public bool login(string username, string password, string catpcha)
        {
            HttpRequest request = HttpRequest.getInstance();
            string result = "";
            string url = "";
            Hashtable param = new Hashtable(); 

            url = "http://192.168.0.12:8080/ibts/j_spring_security_check";
            param.Clear();
            param.Add("j_organization_code", "10000201");
            param.Add("j_username", username);
            param.Add("j_password", password);
            param.Add("j_verifycode", catpcha); 
            result = request.doPost(url, param);
            

            //url = "http://192.168.0.12:8080/ibts/user_authSuccess.action";
            //param.Clear();
            //result = request.doPost(url, param);

            //url = "http://192.168.0.12:8080/ibts/main.action";
            //param.Clear();
            //result = request.doPost(url, param);    

            return true;
        }

        public Stream getCapchta()
        {
            HttpRequest request = HttpRequest.getInstance(); 
            string url = "";
            Hashtable param = new Hashtable();

            url = "http://192.168.0.12:8080/ibts/validation_generatorCode.action";
            param.Add(new DateTime().ToString(), "1"); 
            return request.doPostS(url, param); 
        }

        public string getTaskList()
        {
            HttpRequest request = HttpRequest.getInstance();
            string result = "";
            string url = "";
            Hashtable param = new Hashtable();

            url = "http://192.168.0.12:8080/ibts/task.szDateCenterTesterfindTaskInfo.action";  
            // http://192.168.0.12:8080/ibts/task.findUserCompletedTask.action

            param.Clear();
            param.Add("stage", "6");
            param.Add("state", "1");
            param.Add("pageSize", "10");
            param.Add("pageNo", "1");
            result = request.doPost(url, param);

            return result;
        }
    }
}