==============================================
3Dconnexion plug-in for Blender v0.1 (Windows)
==============================================

Purpose
-------

This software distribution enables the use of 3Dconnexion devices (such as the SpaceNavigator) with an experimental version of Blender on the Microsoft Windows platform.

How
---

1. Download and install the latest 3Dconnexion driver from:
http://www.3dconnexion.com

2. Download the zip file of the experimental build of Blender from either one of these web-sites:
http://www.blenderbuilds.com
http://www.graphicall.org

3. Uncompress the above zip file (from step 2). You should find a "plugins" folder next to the main "blender-NDOF" executable. If it's not there, create it.

4. Copy the 3DxNdofBlender.plug file into the aforementioned "plugins" folder. You should then have the following directory structure (the names could be slightly different):

blender-NDOF (dir)
   +- blender-NDOF (executable)
   +- plugins (dir)
         +- 3DxNdofBlender.plug

5. Launch the 3Dconnexion driver or make sure it is running.

6. Launch the blender-NDOF executable.

7. Go to the 3Dconnexion driver preferences window. In the Buttons Mapping tab, map the Left and Right buttons of the SpaceNavigator to:
	Button 1
	Button 2
If you are using another 3Dconnexion device, simply make sure that the buttons labeled "1" and "2" are associated to the functions "Button 1" and "Button 2" in the relative drop down menus.
Then click the "Save" button and provide a meaningful description for your custom configuration. If you already have one, you can add these settings to that.

8. You should now be able to use the device inside blender, including buttons to toggle modes (turntable/fly/transform) and constraints (unconstrained/dominant).

System Requirements
-------------------

All the requirements of the main blender application apply.

Got Problems?
-------------

Please understand that this software is experimental and it did not receive extensive testing. It is provided as "alpha" software in the hope it is useful to the community.

You're invited to report issues and give suggestions to make this software better to the following forum:
http://www.3dconnexion.com/forum/

License
-------

This software is covered by the LGPL license v2.1. Refer to the COPYING and COPYING.LESSER text files.

Disclaimer
----------

The full disclaimer of the accompanying LGPL license applies. Please read the COPYING and COPYING.LESSER files included in this software distribution.

History of Changes
------------------

v0.1 - 31 July 2007
- Initial version.



________________________________
Copyright 2007 3Dconnexion, Inc.
