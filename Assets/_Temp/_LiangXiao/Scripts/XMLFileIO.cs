using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// xml file read and write
/// </summary>
public class XMLFileIO
{
    /// <summary>
    /// read xml file with specific file path and selected element name
    /// </summary>
    /// <param name="filePath">file path</param>
    /// <param name="elementName">main element name, read all element if empty or null</param>
    /// <returns></returns>
    public static List<Dictionary<string, string>> ReadXML(string filePath, string elementName = "")
    {
        try
        {
            //check file not exist
            if (!File.Exists((filePath)))
            {
                Debug.LogError(filePath + " is not exist, read XML aborted!");
                return null;
            }

            //load file
            XDocument xml = XDocument.Load(filePath);

            //select specific elements
            var elements = string.IsNullOrEmpty(elementName) ? xml.Root.Descendants() : xml.Root.Descendants(elementName);

            var loadedList = new List<Dictionary<string, string>>();

            //ergodic elements
            foreach (var element in elements)
            {
                var loadedDic = new Dictionary<string, string>();

                //ergodic attributes
                foreach (var attribute in element.Attributes())
                {
                    //add attribute data
                    loadedDic.Add(attribute.Name.ToString(), attribute.Value);
                }

                loadedList.Add(loadedDic);
            }

            //check for empty loaded data.
            if (loadedList == null || loadedList.Count == 0)
            {
                Debug.LogWarning("Loaded xml data is empty, please check.");
            }

            Debug.Log("File imported.");

            return loadedList;
        }
        catch (Exception e)
        {
            Debug.LogError("Got exception when load xml file: " + filePath + "\nMessage: " + e.Message + "\nStackTrace: " + e.StackTrace);
            return null;
        }
    }

    /// <summary>
    /// write data to xml file with specific file path and selected element name
    /// </summary>
    /// <param name="filePath">file path</param>
    /// <param name="parsedXML">data to write</param>
    /// <param name="elementName">main element name</param>
    public static void WriteXML(string filePath, List<Dictionary<string, string>> parsedXML, string elementName)
    {
        //check file exist
        if (File.Exists((filePath)))
        {
            Debug.LogWarning(filePath + " is existed, XML will be replaced!");
        }

        //create document
        var document = new XDocument(new XElement("root"));

        //Write data to file, Ergodic elements
        foreach (var element in parsedXML)
        {
            var xElement = new XElement(elementName);

            //Ergodic attributes.
            foreach (var attribute in element)
            {
                //add attribute to element
                xElement.Add(new XAttribute(attribute.Key, attribute.Value));
            }

            document.Root.Add(xElement);
        }

        //save document
        document.Save(filePath);
        Debug.Log("File exported.");
    }
}
