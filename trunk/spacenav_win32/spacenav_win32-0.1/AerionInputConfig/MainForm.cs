/*
   AerionInputConfig Main Form
 
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TDxInput;

namespace AerionInputConfig
{
    public partial class MainForm : Form
    {
        private Preferences old_prefs;
        private Preferences new_prefs;

        public MainForm()
        {
            InitializeComponent();
            old_prefs = new Preferences();
            old_prefs.loadConfig();

            new_prefs = new Preferences();
            new_prefs.loadConfig();

            fillApplications(new_prefs);
            fillFields(new_prefs, 0);
            ApplicationCombo.SelectedIndex = 0;            
        }

        private void fillApplications(Preferences p_pref)
        {
            for (p_pref.index = 0; p_pref.index < p_pref.Count; p_pref.index++)
            {
                ApplicationCombo.Items.Add(p_pref.PreferencesName);
            }
        }

        private void fillFields(Preferences p_pref, int index)
        {
            p_pref.index = index;

            //Translation
            XScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.XScaleFactor);
            YScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.YScaleFactor);
            ZScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.ZScaleFactor);
            TranslationThreshold.Value = System.Convert.ToDecimal(p_pref.TranslationThreshold);
            TranslationFunctionCombo.SelectedIndex = System.Convert.ToInt32(p_pref.TranslationFunction);

            //Rotation
            XRotScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.XRotScaleFactor);
            YRotScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.YRotScaleFactor);
            ZRotScaleFactorBox.Value = System.Convert.ToDecimal(p_pref.ZRotScaleFactor);
            RotationThreshold.Value = System.Convert.ToDecimal(p_pref.RotationThreshold);
            RotationFunctionCombo.SelectedIndex = System.Convert.ToInt32(p_pref.RotationFunction);
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            SaveValues();
            new_prefs.saveConfig();
            Application.Exit();            
        }

        private void SaveValues()
        {
            new_prefs.XScaleFactor = System.Convert.ToDouble(XScaleFactorBox.Value);
            new_prefs.YScaleFactor = System.Convert.ToDouble(YScaleFactorBox.Value);
            new_prefs.ZScaleFactor = System.Convert.ToDouble(ZScaleFactorBox.Value);
            new_prefs.TranslationThreshold = System.Convert.ToDouble(TranslationThreshold.Value);
            new_prefs.TranslationFunction = (TDxInput.functions) System.Convert.ToInt32(TranslationFunctionCombo.SelectedIndex);

            new_prefs.XRotScaleFactor = System.Convert.ToDouble(XRotScaleFactorBox.Value.ToString());
            new_prefs.YRotScaleFactor = System.Convert.ToDouble(YRotScaleFactorBox.Value.ToString());
            new_prefs.ZRotScaleFactor = System.Convert.ToDouble(ZRotScaleFactorBox.Value.ToString());
            new_prefs.RotationThreshold = System.Convert.ToDouble(RotationThreshold.Value.ToString());
            new_prefs.RotationFunction = (TDxInput.functions) System.Convert.ToInt32(RotationFunctionCombo.SelectedIndex);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            fillFields(old_prefs, ApplicationCombo.SelectedIndex);
        }

        private void ApplicationCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveValues();
            fillFields(new_prefs, ApplicationCombo.SelectedIndex);
        }
    }
}