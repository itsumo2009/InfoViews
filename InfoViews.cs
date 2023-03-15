using ICities;
using System.IO;
using InfoViews.Util;
using ColossalFramework.UI;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace InfoViews
{
    public class InfoViews : IUserMod
    {
        private readonly Vector2 basePointFirst = new Vector2(4, 654 + 37);
        private readonly Vector2 basePointSecond = new Vector2(41, 654 + 37);

        public enum ExtendedMode
        {
            None
        }

        public static ExtendedMode mode;

        public static bool IsEnabled = false;

        public string Name
        {
            get { return "Инфопросмотры"; }
        }

        public string Description
        {
            get { return "Получайте обзорную информацию о вакансиях, заполненности учебных заведений, кладбищ и свалок."; }
        }

        public void OnEnabled()
        {
            IsEnabled = true;
            FileStream fs = File.Create("InfoViews.txt");
            fs.Close();
        }

        public void OnDisabled()
        {
            IsEnabled = false;
        }
    }
}

