﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;



namespace Misp.Kernel.Util
{
    /// <summary>
    /// Cette classe permet de gérer les interactions avec le presse-papier du système
    /// avec des méthodes permettant de vérifier l'état du clipboard vide ou pas,
    /// de récupérer le type de d'objets présent dans le presse-papiers.
    /// 
    /// </summary>
    public class ClipbordUtil
    {
        #region Attributs statiques
        /// <summary>
        /// Les types d'objet sur lesquels on peut réaliser des opérations de copie,coupe, coller
        /// </summary>
        public static string GROUP_CLIPBOARD_FORMAT = "Misp.BGroup";
        public static string MEASURE_CLIPBOARD_FORMAT = "Misp.Measure";
        public static string ROLE_CLIPBOARD_FORMAT = "Misp.Role";
        public static string ATTRIBUTE_CLIPBOARD_FORMAT = "Misp.Attribute";
        public static string PERIODNAME_CLIPBOARD_FORMAT = "Misp.PeriodName";
        public static string ATTRIBUTE_VALUE_CLIPBOARD_FORMAT = "Misp.Value";
        public static string TARGET_VALUE_CLIPBOARD_FORMAT = "Misp.Target";
        public static string RANGE_CLIPBOARD_FORMAT = "Misp.Range";
        public static string IS_PARENT_CHILD_MODE = "IS_PARENT_CHILD_MODE";
        public static int COPY_EXT = 1;
        #endregion
        
        
        #region Operations

        /// <summary>
        /// Cette méthode vérifie si le presse-papiers contient du texte ou
        /// un type de données spécifique.
        /// </summary>
        /// <returns>false=> si l'on trouve un des types spécifiés, true=>sinon</returns>
        public static bool IsClipBoardEmpty()
        {
             bool res;
             if (System.Windows.Clipboard.ContainsText()) res = false;
             else if (System.Windows.Clipboard.ContainsData(TARGET_VALUE_CLIPBOARD_FORMAT)) res = false;
             else if (System.Windows.Clipboard.ContainsData(GROUP_CLIPBOARD_FORMAT)) res = false;
             else if (System.Windows.Clipboard.ContainsData(MEASURE_CLIPBOARD_FORMAT)) res = false;
             else if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_CLIPBOARD_FORMAT)) res = false;
             else if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT)) res = false;
             else res = true;
             return res;
        }

        public static bool IsClipBoardEmptyPeriodName()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsText()) res = false;
            else if (System.Windows.Clipboard.ContainsData(PERIODNAME_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }

        public static bool IsClipBoardEmptyAttributeValue()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsText()) res = false;
            else if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }

        public static bool IsClipBoardEmptyAttribute()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsText()) res = false;
            else if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }

        public static bool IsClipBoardEmptyMeasure()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsText()) res = false;
            else if (System.Windows.Clipboard.ContainsData(MEASURE_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }

        public static bool IsClipBoardEmptyGroup()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsText()) res = false;
            else if (System.Windows.Clipboard.ContainsData(GROUP_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }
        
        public static bool IsClipBoardEmptyRange()
        {
            bool res;
            if (System.Windows.Clipboard.ContainsData(RANGE_CLIPBOARD_FORMAT)) res = false;
            else res = true;
            return res;
        }
        
        public static void SetDatas(String format, List<Object> datas)
        {
            if (format == null || datas == null || datas.Count == 0) return;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(datas);
            System.Windows.Clipboard.SetData(format, json);
        }

        public static List<Object> GetTextDatas(string format)
        {
            List<Object> datas = new List<Object>(0);
            if (format != null && System.Windows.Clipboard.ContainsText())
            {
                string name = System.Windows.Clipboard.GetText();
                if (format.Equals(GROUP_CLIPBOARD_FORMAT))
                {
                    foreach (string copies in ExcelCellsCopies(name))
                    {
                        Domain.BGroup group = new Domain.BGroup();
                        group.name = copies;
                        group.subjectType = Domain.SubjectType.DEFAULT.label;
                        datas.Add(group as Domain.BGroup);
                    }
                }

                else if (format.Equals(MEASURE_CLIPBOARD_FORMAT))
                {
                    Kernel.Domain.Measure parent = null;
                    List<String> listeResult = ExcelCellsCopies(name);
                    if (listeResult == null) return null;
                    if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    {
                        parent = new Domain.Measure()
                        {
                            name = listeResult[1]
                        };
                        listeResult.RemoveAt(0);
                        listeResult.RemoveAt(0);
                    }
                    foreach (string copies in listeResult)
                    {
                        Domain.Measure value = new Domain.Measure();
                        value.name = copies.Trim();
                        if (parent != null) parent.AddChild(value as Domain.Measure);
                        else datas.Add(value as Domain.Measure);
                    }
                    if (parent != null) datas.Add(parent as Domain.Measure);
                }

                else if (format.Equals(ATTRIBUTE_CLIPBOARD_FORMAT))
                {
                    List<String> listeResult = ExcelCellsCopies(name);
                    if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    {
                        listeResult.RemoveAt(0);
                    }
                    foreach (string copies in listeResult)
                    {
                        Domain.Attribute attribute = new Domain.Attribute();
                        attribute.name = copies;
                        datas.Add(attribute as Domain.Attribute);
                    }
                }
                
                else if (format.Equals(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT))
                {
                    Kernel.Domain.AttributeValue parent = null;

                    List<String> listeResult = ExcelCellsCopies(name);
                    if (listeResult == null) return null;
                    if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    {
                        parent = new Domain.AttributeValue()
                        {
                            name = listeResult[1]
                        };
                        listeResult.RemoveAt(0);
                        listeResult.RemoveAt(0);
                    }
                    foreach (string copies in listeResult)
                    {
                        Domain.AttributeValue value = new Domain.AttributeValue();
                        value.name = copies;
                        if (parent != null) parent.AddChild(value as Domain.AttributeValue);
                        else datas.Add(value as Domain.AttributeValue);
                    }
                    if (parent != null) datas.Add(parent as Domain.AttributeValue);
                }
            }
            return datas;
        }

        public static void SetMeasures(List<Object> datas)
        {
            SetDatas(MEASURE_CLIPBOARD_FORMAT, datas);
        }

        public static void SetRoles(List<Object> datas)
        {
            SetDatas(ROLE_CLIPBOARD_FORMAT, datas);
        }
        
        public static void SetGroups(List<Object> datas)
        {
            SetDatas(GROUP_CLIPBOARD_FORMAT, datas);
        }

        public static void SetAttributeValues(List<Object> datas)
        {
            SetDatas(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT, datas);
        }

        public static void SetAttributes(List<Object> datas)
        {
            SetDatas(ATTRIBUTE_CLIPBOARD_FORMAT, datas);
        }

        public static void SetPeriodNames(List<Object> datas)
        {
            SetDatas(PERIODNAME_CLIPBOARD_FORMAT, datas);
        }


        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient un group et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.BGroup> GetGroups()
        {
            List<Domain.IHierarchyObject> listeGroup = GetHierarchyObject(GROUP_CLIPBOARD_FORMAT);
            if (listeGroup != null)
            {
                List<Domain.BGroup> ob = listeGroup.Cast<Domain.BGroup>().ToList();
                if (ob != null && ob.Count > 0) return ob;
            }
            return null;
        }

        public static List<Domain.BGroup> GetGroupes()
        {
            if (System.Windows.Clipboard.ContainsData(GROUP_CLIPBOARD_FORMAT))
            {
                try
                {
                    object data = System.Windows.Clipboard.GetData(GROUP_CLIPBOARD_FORMAT);
                    if (data != null && data is String) return RestSharp.SimpleJson.DeserializeObject<List<Domain.BGroup>>((String)data);
                }
                catch (Exception)
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Error copy", "Unable to paste " + GROUP_CLIPBOARD_FORMAT.Split('.')[1]);
                }
            }
            else if (System.Windows.Clipboard.ContainsText())
            {
                List<Domain.BGroup> groups = GetTextDatas(GROUP_CLIPBOARD_FORMAT).Cast<Domain.BGroup>().ToList();
                if (groups != null) return groups;
            }
            return new List<Domain.BGroup>(0);
        }

        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient une mesure et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.Measure> GetMeasures()
        {
            if (System.Windows.Clipboard.ContainsData(MEASURE_CLIPBOARD_FORMAT))
            {
                try
                {
                    object data = System.Windows.Clipboard.GetData(MEASURE_CLIPBOARD_FORMAT);
                    if(data != null && data is String) return RestSharp.SimpleJson.DeserializeObject<List<Domain.Measure>>((String)data);
                }
                catch (Exception)
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Error copy", "Unable to paste " + MEASURE_CLIPBOARD_FORMAT.Split('.')[1]);
                }
            }
            else if (System.Windows.Clipboard.ContainsText())
            {
                List<Domain.Measure> measures = GetTextDatas(MEASURE_CLIPBOARD_FORMAT).Cast<Domain.Measure>().ToList();
                if (measures != null) return measures;                
            }
            return new List<Domain.Measure>(0);
        }

        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient une role et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.Role> GetRole()
        {
            List<Domain.IHierarchyObject> listeRole = GetHierarchyObject(MEASURE_CLIPBOARD_FORMAT);
            if (listeRole != null)
            {
                List<Domain.Role> ob = listeRole.Cast<Domain.Role>().ToList();
                if (ob != null && ob.Count > 0) return ob;
            }
            return null;
        }

        public static List<Domain.PeriodName> GetPeriodName()
        {
            try
            {
                List<Domain.IHierarchyObject> listeAttribute = GetHierarchyObject(PERIODNAME_CLIPBOARD_FORMAT);
                if (listeAttribute != null)
                {
                    List<Domain.PeriodName> ob = listeAttribute.Cast<Domain.PeriodName>().ToList();
                    if (ob != null && ob.Count > 0) return ob;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient une Attribute et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.Attribute> GetAttributes()
        {
            if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_CLIPBOARD_FORMAT))
            {
                try
                {
                    object data = System.Windows.Clipboard.GetData(ATTRIBUTE_CLIPBOARD_FORMAT);
                    if (data != null && data is String) return RestSharp.SimpleJson.DeserializeObject<List<Domain.Attribute>>((String)data);
                }
                catch (Exception)
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Error copy", "Unable to paste " + ATTRIBUTE_CLIPBOARD_FORMAT.Split('.')[1]);
                }
            }
            else if (System.Windows.Clipboard.ContainsText())
            {
                List<Domain.Attribute> attributes = GetTextDatas(ATTRIBUTE_CLIPBOARD_FORMAT).Cast<Domain.Attribute>().ToList();
                if (attributes != null) return attributes;
            }
            return new List<Domain.Attribute>(0);
        }

        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient une AttributeValue et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.AttributeValue> GetAttributeValues()
        {
            if (System.Windows.Clipboard.ContainsData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT))
            {
                try
                {
                    object data = System.Windows.Clipboard.GetData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT);
                    if (data != null && data is String) return RestSharp.SimpleJson.DeserializeObject<List<Domain.AttributeValue>>((String)data);
                }
                catch (Exception)
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Error copy", "Unable to paste " + ATTRIBUTE_VALUE_CLIPBOARD_FORMAT.Split('.')[1]);
                }
            }
            else if (System.Windows.Clipboard.ContainsText())
            {
                List<Domain.AttributeValue> values = GetTextDatas(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT).Cast<Domain.AttributeValue>().ToList();
                if (values != null) return values;
            }
            return new List<Domain.AttributeValue>(0);
        }

        /// <summary>
        /// Cette méthode permet de vider le contenu du presse-papier.
        /// </summary>
        public static void ClearClipboard()
        {
            System.Windows.Clipboard.Clear();
        }
              









        /// <summary>
        /// Cette méthode insère un type de données spécifique dans le presse-papiers
        /// </summary>
        /// <param name="item">Un objet du type à insérer dans la presse-papiers</param>
        public static void SetHierarchyObject(Domain.IHierarchyObject item)
        {
            if (item == null) return;
            if (item is Domain.BGroup) System.Windows.Clipboard.SetData(GROUP_CLIPBOARD_FORMAT, item);
            if (item is Domain.Measure) System.Windows.Clipboard.SetData(MEASURE_CLIPBOARD_FORMAT, item);
            if (item is Domain.Attribute) System.Windows.Clipboard.SetData(ATTRIBUTE_CLIPBOARD_FORMAT, item);
            if (item is Domain.PeriodName) System.Windows.Clipboard.SetData(PERIODNAME_CLIPBOARD_FORMAT, item);
            if (item is Domain.AttributeValue) System.Windows.Clipboard.SetData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT, item);
        }
        
        /// <summary>
        /// Cette méthode insère un type de données spécifique dans le presse-papiers
        /// </summary>
        /// <param name="item">Une liste d'objet du type à insérer dans la presse-papiers</param>
        public static void SetHierarchyObject(List<Kernel.Domain.IHierarchyObject> listeItem)
        {
            if (listeItem == null) return;
            if (listeItem.Count > 0) 
            {
                Kernel.Domain.IHierarchyObject item = listeItem[0];
                if (item is Kernel.Domain.BGroup) System.Windows.Clipboard.SetData(GROUP_CLIPBOARD_FORMAT, listeItem);
                if (item is Domain.Measure) System.Windows.Clipboard.SetData(MEASURE_CLIPBOARD_FORMAT, listeItem);
                if (item is Domain.Attribute) System.Windows.Clipboard.SetData(ATTRIBUTE_CLIPBOARD_FORMAT, listeItem);
                if (item is Domain.AttributeValue) System.Windows.Clipboard.SetData(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT, listeItem);
                if (item is Domain.PeriodName) System.Windows.Clipboard.SetData(PERIODNAME_CLIPBOARD_FORMAT, listeItem);
            }
        }
        
        /// <summary>
        /// Mets les ranges selectionnés dans le presse-papier.
        /// </summary>
        /// <param name="range"></param>
        public static void SetRange(Ui.Office.Range range) 
        {
            if (range == null) return;
            ClearClipboard();
            System.Windows.Clipboard.SetData(RANGE_CLIPBOARD_FORMAT, range);
        }

        /// <summary>
        /// Récupère les ranges du presse-papier.
        /// </summary>
        /// <returns></returns>
        public static Ui.Office.Range getRange() 
        {
            if (System.Windows.Clipboard.ContainsData(RANGE_CLIPBOARD_FORMAT))
            return System.Windows.Clipboard.GetData(RANGE_CLIPBOARD_FORMAT) as Ui.Office.Range;
             return null;
        }
        
        /// <summary>
        /// Cette méthode détermine le nombre de colonnes copiées depuis
        /// un fichier excel.
        /// Elle prend en paramètre une chaine de caractère et vérifie si celle -ci
        /// contient le séparateur de colonne excel '\t'.
        /// </summary>
        /// <param name="chaine">la chaine copiée dans le presse papier</param>
        /// <returns>Le nombre de colonne copiées</returns>
        public static int ColonneCopiees(string chaine)
        {
            int result = 0;
            foreach (char c in chaine)
            {
                if (c == '\t')
                    result++;
            }
            return result;
        }

        /// <summary>
        /// Renvoit la liste des éléments copiés dans une colonne Excel 
        /// </summary>
        /// <returns>La liste des Targets copiés.</returns>
        public static List<String> ExcelCellsCopies(String donnees)
        {
            List<String> listResult = new List<String>(0);

            if (!IsClipBoardEmpty())
            {
                String[] lignes = donnees.Split('\n');

                int nbreColonnes = ColonneCopiees(lignes[0]) + 1;
                if (nbreColonnes > 2) return null;
                if (nbreColonnes == 1)
                {
                    listResult = parcourCells(lignes);
                }
                else if(nbreColonnes == 2)
                { 
                   //Mode père fils
                    if (lignes.Length == 0) return null;
                    object[] cellparent = lignes[0].Split('\t');
                    listResult.Add(IS_PARENT_CHILD_MODE);
                    if(!String.IsNullOrEmpty(cellparent[0].ToString().Trim()))
                        listResult.Add(cellparent[0].ToString().Trim());
                    List<String> sons = parcourCells(lignes,1);
                    listResult.AddRange(sons);

                }
            }
            return listResult.Count == 0 ? null : listResult;
        }

        public Dictionary<String, List<String>> listeElements { get; set; }

        /// <summary>
        /// Renvoit la liste des éléments copiés dans une colonne Excel 
        /// </summary>
        /// <returns>La liste des Targets copiés.</returns>
        public static List<Kernel.Domain.AttributeValue> ExcelGetCellsCopies(String donnees, String format)
        {
            List<Kernel.Domain.AttributeValue> listResult = new List<Kernel.Domain.AttributeValue>(0);

            if (!IsClipBoardEmpty())
            {
                String[] lignes = donnees.Split('\r');
                if (lignes.Length == 0) return null;
                int nbreColonnes = ColonneCopiees(lignes[0]) + 1;
                if (nbreColonnes > 2) return null;
                if (nbreColonnes == 1)
                {
                    object[] cellfirst = lignes[0].Split('\n');
                    addParent(cellfirst[0].ToString().Trim(),ref listResult);
                    int j = 0;
                    int indexParent = 0;
                    for (int i = lignes.Length - 1; i >= 1; i--) 
                    {
                        if (lignes[j].StartsWith("\n"))
                        {
                            List<Kernel.Domain.AttributeValue> sons = new List<Domain.AttributeValue>(0);
                            addParent(lignes[j].ToString().Trim(), ref sons);
                            if(sons.Count != 0) 
                            listResult[indexParent].childrenListChangeHandler.AddNew(sons, false);
                        }
                        else addParent(lignes[i].ToString().Trim(), ref listResult);
                        int isChild =  lignes[i].Count(x => x == '\n');
                        j++;
                    }
                    //Mode père fils
                    if (lignes.Length == 0) return null;
                    object[] cellparent = lignes[0].Split("\n".ToCharArray());
                }
            }
            return listResult.Count == 0 ? null : listResult;
        }
      
        static void addParent(String name, ref List<Kernel.Domain.AttributeValue> liste ) 
        {
            if (!String.IsNullOrEmpty(name))
            {
                Kernel.Domain.AttributeValue parent = new Kernel.Domain.AttributeValue()
                {
                    name = name
                };
                liste.Add(parent);
            }
        }
        private static List<Kernel.Domain.AttributeValue> parcourCellsSub(String[] lignes, int index = 0)
        {
            List<Kernel.Domain.AttributeValue> listResult = new List<Kernel.Domain.AttributeValue>(0);
            for (int i = 0; i < lignes.Length; i++)
            {
                object[] cells = lignes[i].Split("\n".ToCharArray());
                if (index < cells.Length)
                {
                    if (!String.IsNullOrEmpty(cells[index].ToString().Trim()))
                    {
                        //String addString = cells[index].ToString().Trim();
                        //if (listResult.Contains(addString))
                        //    addString += (COPY_EXT++);
                        //listResult.Add(addString);
                    }
                }
            }
            return listResult;
        }

        private static List<String> parcourCells(String[] lignes,int index=0) 
        {
            List<String> listResult = new List<String>(0);
            for (int i = 0; i < lignes.Length; i++)
            {
                object[] cells = lignes[i].Split('\t');
                if (index < cells.Length)
                {
                    if (!String.IsNullOrEmpty(cells[index].ToString().Trim()))
                    {
                        String addString = cells[index].ToString().Trim();
                        if (listResult.Contains(addString))
                            addString += (COPY_EXT++);
                        listResult.Add(addString);
                    }
                }
            }
            return listResult;
        }

        /// <summary>
        /// Cette méthode vérifie si le presse-papier contient un type de donnés et 
        /// renvoie un objet de ce type
        /// </summary>
        /// <param name="format">Le format du type de données</param>
        /// <returns>L'objet présent dans le presse-papiers</returns>
        public static List<Domain.IHierarchyObject> GetHierarchyObject(string format)
        {
            List<Domain.IHierarchyObject> copiedElements = new List<Domain.IHierarchyObject>(0);
            if (format == null) return copiedElements;
            if (System.Windows.Clipboard.ContainsData(format))
            {
                try
                {
                    if (System.Windows.Clipboard.GetData(format) is IList)
                        copiedElements = (List<Domain.IHierarchyObject>)System.Windows.Clipboard.GetData(format);
                    else
                        copiedElements.Add(System.Windows.Clipboard.GetData(format) as Domain.IHierarchyObject);
                    return copiedElements;
                }
                catch (Exception)
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Error copy", "Unable to paste "+format.Split('.')[1]);
                    return null;
                }
            }
            else if (System.Windows.Clipboard.ContainsText())
            {
                string name = System.Windows.Clipboard.GetText();
                if (format.Equals(GROUP_CLIPBOARD_FORMAT))
                {
                    foreach (string copies in ExcelCellsCopies(name))
                    {
                        Domain.BGroup group = new Domain.BGroup();
                        group.name = copies;
                        group.subjectType = Domain.SubjectType.DEFAULT.label;
                        copiedElements.Add(group as Domain.BGroup);
                    }
                    return copiedElements;
                }
                                
                if (format.Equals(ATTRIBUTE_CLIPBOARD_FORMAT))
                {
                    List<String> listeResult = ExcelCellsCopies(name);
                    if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    {
                        listeResult.RemoveAt(0);
                    }
                    foreach (string copies in listeResult)
                    {
                        Domain.Attribute attribute = new Domain.Attribute();
                        attribute.name = copies;
                        copiedElements.Add(attribute as Domain.Attribute);
                    }
                    return copiedElements;
                }
                if (format.Equals(ATTRIBUTE_VALUE_CLIPBOARD_FORMAT))
                {
                    Kernel.Domain.AttributeValue parent = null;

                    List<String> listeResult = ExcelCellsCopies(name);
                    if (listeResult == null) return null;
                    if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    {
                        parent = new Domain.AttributeValue()
                        {
                            name = listeResult[1]
                        };
                        listeResult.RemoveAt(0);
                        listeResult.RemoveAt(0);
                    }
                    foreach (string copies in listeResult)
                    {
                        Domain.AttributeValue value = new Domain.AttributeValue();
                        value.name = copies;
                        if (parent != null) parent.AddChild(value as Domain.AttributeValue);
                        else copiedElements.Add(value as Domain.AttributeValue);
                    }
                    if (parent != null) copiedElements.Add(parent as Domain.AttributeValue);
                    return copiedElements;

                 //   List<String> listeResult = ExcelGetCellsCopies(name);
                   // List<Kernel.Domain.AttributeValue> Liste = ExcelGetCellsCopies(name, ATTRIBUTE_VALUE_CLIPBOARD_FORMAT);
                    //if (listeResult == null) return null;
                    //if (listeResult[0] == IS_PARENT_CHILD_MODE)
                    //{
                    //    parent = new Domain.AttributeValue() 
                    //    {
                    //     name = listeResult[1]
                    //    };
                    //    listeResult.RemoveAt(0);
                    //    listeResult.RemoveAt(0);
                    //}
                    //foreach (string copies in listeResult)
                    //{
                    //    Domain.AttributeValue value = new Domain.AttributeValue();
                    //    value.name = copies;
                    //    if (parent != null) parent.AddChild(value as Domain.AttributeValue);
                    //    else copiedElements.Add(value as Domain.AttributeValue);
                    //}
                    //if (parent != null) copiedElements.Add(parent as Domain.AttributeValue);
                    //return copiedElements;
                }
            }
            return null;
        }

        #endregion
    }
}
