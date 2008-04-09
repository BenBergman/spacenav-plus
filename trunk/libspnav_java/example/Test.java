/*
This file is part of libspnav, part of the spacenav project (spacenav.sf.net)
Copyright (C) 2007 John Tsiombikas <nuclear@siggraph.org>
Copyright (C) 2008 Michael Arndt <scriptkiller@gmx.de>

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
OF SUCH DAMAGE.
*/

import net.sf.spacenav.*;

public class Test {

  public static void main(String args[]) {


    System.out.println("Starting Test, press button 0 to exit");

    SpaceNav s=new SpaceNav();
    s.setSensitivity(1.0);
    SpaceNavEvent e;
    while((e=s.waitForEvent())!=null) {
      if(e instanceof SpaceNavMotionEvent) {
	SpaceNavMotionEvent m=(SpaceNavMotionEvent) e;
	System.out.printf("Motion event: tx:%4d ty:%4d tz:%4d rx:%4d ry:%4d rz:%4d\n",
	    m.getX(), m.getY(), m.getZ(),
	    m.getRX(), m.getRY(), m.getRZ()
	);
      }
      else if(e instanceof SpaceNavButtonEvent) {
	SpaceNavButtonEvent b=(SpaceNavButtonEvent) e;
	System.out.printf("Button event: %d %s\n",
	    b.getButton(), b.isPressed() ? "pressed" : "released" );

	if(b.getButton()==0 && b.isPressed())
	  break;
      }
      else {
	System.out.println("Unknown event!");
      }
    }
    s.closeDevice();
  }

}
