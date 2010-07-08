/*
   Device class implementation
 
   Copyright (C)    Christian Bayer 2008
					Friedrich-Schiller University of Jena
					Bauhaus University of Weimar

   This library is free software; you can redistribute it and/or
   modify it under the terms of the GNU Lesser General Public
   License as published by the Free Software Foundation; either
   version 2.1 of the License, or (at your option) any later version.

   This library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public
   License along with this library; if not, see http://www.gnu.org/licenses .

*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace TDxInput
{
    /// <summary>
    /// Enumeration of possible functions in order to save numeric values to
    /// the config file (and not delegates..)
    /// </summary>
    public enum functions
    {
        none,
        sin,
        square,
        cube
    }

    /// <summary>
    /// Represents a configuration profile on a per-application basis.
    /// In order to load another profile just use the Setter of PreferencesName. If a
    /// profile of this name exists it will be loaded, if not a new default profile of this
    /// name will be created.
    /// This class is an extension of the orginal API, there is no such class exposed to COM.
    /// Thatswhy the Attribute ComVisible(false).
    /// So this is our implementation of the LoadPreferencesName() call of Device.
    /// The documentation says nothing about that so we can only guess the real functionality.
    /// This is our "educated" guess so ;)
    /// </summary>
    [ComVisible(false)]
	public class Preferences
    {
        ////////////////////
        // Private fields //
        ////////////////////

        private List <functions> m_RotationFunction = new List<functions>();
        private List <functions> m_TranslationFunction = new List<functions>();
        private List <string> m_PreferencesName = new List<string>();
        private List <double> m_XRotScaleFactor = new List<double>();
        private List <double> m_YRotScaleFactor = new List<double>();
        private List <double> m_ZRotScaleFactor = new List<double>();
        private List <double> m_XScaleFactor = new List<double>();
        private List <double> m_YScaleFactor = new List<double>();
        private List <double> m_ZScaleFactor = new List<double>();
        private List <double> m_TranslationThreshold = new List<double>();
        private List <double> m_RotationThreshold = new List<double>();
        
        private int m_index;
        /// <summary>
        /// Contains the exact path (with file name) to the XML config file.
        /// </summary>
        private string path;

        ///////////////////////////
        // Getter/Setter methods //
        ///////////////////////////

        /// <summary>
        /// Gets or Sets the filter function which shall be applied to the rotational input data.
        /// </summary>
        public functions RotationFunction
        {
            get
            {
                return m_RotationFunction[m_index];
            }

            set
            {
                m_RotationFunction[m_index] = value;
            }

        }

        /// <summary>
        /// Gets or Sets the list index of the current profile.
        /// </summary>
        public int index
        {

            get
            {
                return m_index;
            }

            set
            {
                m_index = value;
            }

        }

        /// <summary>
        /// Gets or Sets the Translation Filter Function of the current profile.
        /// Possible values: sin, cos, square, cube, none.
        /// </summary>
        public functions TranslationFunction
{

            get
            {
                return m_TranslationFunction[m_index];
            }

            set
            {
                m_TranslationFunction[m_index] = value;
            }

        }

        /// <summary>
        /// Gets or Sets the Name of the current profile.
        /// When a name is set which is not in the list holding all known
        /// profiles, it will be created.
        /// </summary>
		public string PreferencesName
        {
            get
            {
                return m_PreferencesName[m_index];
            }

            set
            {
                m_index = m_PreferencesName.IndexOf(value);
                if (m_index == -1)
                {
                    m_PreferencesName.Add(value);
                    m_XScaleFactor.Add(1.0);
                    m_YScaleFactor.Add(1.0);
                    m_ZScaleFactor.Add(1.0);
                    m_TranslationThreshold.Add(0.0);
                    m_TranslationFunction.Add(functions.none);
                    m_XRotScaleFactor.Add(1.0);
                    m_YRotScaleFactor.Add(1.0);
                    m_ZRotScaleFactor.Add(1.0);
                    m_RotationThreshold.Add(0.0);
                    m_RotationFunction.Add(functions.none);                    
                    m_index = m_PreferencesName.Count - 1;
                }
            }
        }

        /// <summary>
        /// Gets the count of stored profiles.
        /// </summary>
        public int Count
        {
            get
            {
                return m_PreferencesName.Count;
            }
        }

        /// <summary>
        /// Gets or Sets the scaling factor the Translation X value will be multiplied with.
        /// </summary>
        public double XScaleFactor
        {
            get
            {
                return m_XScaleFactor[m_index];
            }

            set
            {
                m_XScaleFactor[m_index] = value;
            }
        }

        /// <summary>
        /// Gets or Sets the scaling factor the Translation Y value will be multiplied with.
        /// </summary>
		public double YScaleFactor
		{
			get
			{
                return m_YScaleFactor[m_index];
			}
			
			set
			{
                m_YScaleFactor[m_index] = value;
			}
		}

        /// <summary>
        /// Gets or Sets the scaling factor the Translation Z value will be multiplied with.
        /// </summary>
		public double ZScaleFactor
		{
			get
			{
                return m_ZScaleFactor[m_index];
			}
			
			set
			{
                m_ZScaleFactor[m_index] = value;
			}
		}

        /// <summary>
        /// Gets or Sets the scaling factor the Rotation X value will be multiplied with.
        /// </summary>
		public double XRotScaleFactor
		{			
			get
			{
                return m_XRotScaleFactor[m_index];
			}
			
			set
			{
                m_XRotScaleFactor[m_index] = value;
			}			
		}

        /// <summary>
        /// Gets or Sets the scaling factor the Rotation Y value will be multiplied with.
        /// </summary>
		public double YRotScaleFactor
		{			
			get
			{
                return m_YRotScaleFactor[m_index];
			}
			
			set
			{
                m_YRotScaleFactor[m_index] = value;
			}			
		}

        /// <summary>
        /// Gets or Sets the scaling factor the Rotation Z value will be multiplied with.
        /// </summary>
        public double ZRotScaleFactor
        {
            get
            {
                return m_ZRotScaleFactor[m_index];
            }

            set
            {
                m_ZRotScaleFactor[m_index] = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Rotation threshold value.
        /// </summary>
        public double RotationThreshold
        {
            get
            {
                return m_RotationThreshold[m_index];
            }

            set
            {
                m_RotationThreshold[m_index] = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Translation threshold value.
        /// </summary>
        public double TranslationThreshold
        {
            get
            {
                return m_TranslationThreshold[m_index];
            }

            set
            {
                m_TranslationThreshold[m_index] = value;
            }
        }

        /////////////////////
        // Private methods //
        /////////////////////

        /// <summary>
        /// Creates the path variable to the XML config file. Usually this is 
        /// "C:\Documents and Settings\%username%\Application Data\AerionInput\config.xml".
        /// If directories do not exist, they will be created.
        /// </summary>
        private void ConstructPath()
        {
            path = System.Windows.Forms.Application.UserAppDataPath;
            string[] pathParts = path.Split(System.IO.Path.DirectorySeparatorChar);

            string pathSep = System.Char.ToString(System.IO.Path.DirectorySeparatorChar);
            if (pathParts.Length > 3)
            {
                path = System.String.Join(pathSep, pathParts, 0, pathParts.Length - 3);
                path += "\\AerionInput";
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {

                }
                path += "\\config.xml";
            }
            else throw new Exception("Path String malformed!");
        }

        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Loads the XML config file stored in "path" and parses the config data
        /// to its local variables. If anything goes wrong, all fields will be initialized
        /// with defaults. So if file is malformed, a new valid config file will be created.
        /// </summary>
        public void loadConfig()
		{
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(@path);
                XmlElement Root = doc.DocumentElement;
                XmlNode DataNode;

                // TODO: Found bug: If app name starts with number, no XML node can be read. Ironically it
                // can be written...
                foreach (XmlNode ApplicationNode in Root.ChildNodes)
                {
                    m_PreferencesName.Add(ApplicationNode.Name);
                    
                    //Translation
                    DataNode = ApplicationNode.ChildNodes.Item(0);
                    m_XScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["XScaleFactor"].InnerText));
                    m_YScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["YScaleFactor"].InnerText));
                    m_ZScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["ZScaleFactor"].InnerText));
                    m_TranslationThreshold.Add(System.Convert.ToDouble(DataNode.Attributes["Threshold"].InnerText));
                    m_TranslationFunction.Add((functions)System.Convert.ToInt32(DataNode.Attributes["Function"].InnerText));

                    //Rotation
                    DataNode = ApplicationNode.ChildNodes.Item(1);
                    m_XRotScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["XScaleFactor"].InnerText));
                    m_YRotScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["YScaleFactor"].InnerText));
                    m_ZRotScaleFactor.Add(System.Convert.ToDouble(DataNode.Attributes["ZScaleFactor"].InnerText));
                    m_RotationThreshold.Add(System.Convert.ToDouble(DataNode.Attributes["Threshold"].InnerText));
                    m_RotationFunction.Add((functions)System.Convert.ToInt32(DataNode.Attributes["Function"].InnerText));
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);

                m_PreferencesName = new List<string>();
                m_XScaleFactor = new List<double>();
                m_YScaleFactor = new List<double>();
                m_ZScaleFactor = new List<double>();
                m_TranslationThreshold = new List<double>();
                m_TranslationFunction = new List<functions>();
                m_XRotScaleFactor = new List<double>();
                m_YRotScaleFactor = new List<double>();
                m_ZRotScaleFactor = new List<double>();
                m_RotationThreshold = new List<double>();
                m_RotationFunction = new List<functions>();          
                                                
                m_PreferencesName.Add("default");
                m_XScaleFactor.Add(1.0);
                m_YScaleFactor.Add(1.0);
                m_ZScaleFactor.Add(1.0);
                m_TranslationThreshold.Add(0.0);
                m_TranslationFunction.Add(functions.none);
                m_XRotScaleFactor.Add(1.0);
                m_YRotScaleFactor.Add(1.0);
                m_ZRotScaleFactor.Add(1.0);
                m_RotationThreshold.Add(0.0);
                m_RotationFunction.Add(functions.none);                
                
            }
            m_index = 0;
		}

        /// <summary>
        /// Writes all preferences into XML config file stored in "path".
        /// Must be called explicitly in order to save values for the next session.
        /// Overwrites any existing config file with the values in this object.
        /// </summary>
		public void saveConfig()
		{
            XmlDocument doc = new XmlDocument();
            XmlNode Root, ApplicationNode, DataNode;
            XmlAttribute Attribute;

            Root = doc.CreateElement("AerionInputConfig");
            doc.AppendChild(Root);

            for (int i = 0; i < m_PreferencesName.Count; i++)
            {
                ApplicationNode = doc.CreateElement(m_PreferencesName[i]);
                Root.AppendChild(ApplicationNode);

                DataNode = doc.CreateElement("Translation");

                Attribute = doc.CreateAttribute("XScaleFactor");
                Attribute.InnerText = m_XScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("YScaleFactor");
                Attribute.InnerText = m_YScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("ZScaleFactor");
                Attribute.InnerText = m_ZScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("Threshold");
                Attribute.InnerText = m_TranslationThreshold[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("Function");
                Attribute.InnerText = ((int) m_TranslationFunction[i]).ToString();
                DataNode.Attributes.Append(Attribute);

                ApplicationNode.AppendChild(DataNode);

                DataNode = doc.CreateElement("Rotation");

                Attribute = doc.CreateAttribute("XScaleFactor");
                Attribute.InnerText = m_XRotScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("YScaleFactor");
                Attribute.InnerText = m_YRotScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("ZScaleFactor");
                Attribute.InnerText = m_ZRotScaleFactor[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("Threshold");
                Attribute.InnerText = m_RotationThreshold[i].ToString();
                DataNode.Attributes.Append(Attribute);

                Attribute = doc.CreateAttribute("Function");
                Attribute.InnerText = ((int) m_RotationFunction[i]).ToString();
                DataNode.Attributes.Append(Attribute);

                ApplicationNode.AppendChild(DataNode);            
            }

            try
            {
                doc.Save(@path);
            }
            catch (Exception e)
            {                
               Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);
            }
		}

        /// <summary>
        /// Calls ConstructPath() to have a valid path string
        /// </summary>
        public Preferences()
        {                  
            try 
            {
                ConstructPath();
            }
            catch (Exception e)
            {
               Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);
            }
        }
	}
}
