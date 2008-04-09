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

package net.sf.spacenav;

/**
 * This class provides an interface to the free spacenavd for 3D input
 * devices from 3DConnection (TM),
 * see http://spacenav.sourceforge.net/
 *
 */
public class SpaceNav {

  private native void open();
  private native void close();
  private native void sensitivity(double sens);
  private native SpaceNavEvent wait_event();

  static {
    System.loadLibrary("SpaceNav");
  }

  /**
   * Default constructor, opens the SpaceNav device
   * The connection should be closed with closeDevice() if
   * no longer needed.
   */
  public SpaceNav() {
    /* open device */
    open();
  }


  /**
   * Close the connection to the SpaceNav device
   */
  public void closeDevice() {
    /* close device */
    close();
  }

  /**
   * Set sensitivity of device, seems that
   * the daemon multiplies all values with the supplied
   * sensitivity
   * @param sens the sensitivity
   */
  public void setSensitivity(double sens) {
    sensitivity(sens);
  }

  /**
   * Wait for an event to occur.
   * @return the SpaceNavEvent that occured
   */
  public SpaceNavEvent waitForEvent() {
    return wait_event();
  }

}
