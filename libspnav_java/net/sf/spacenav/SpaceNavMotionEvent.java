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
 * This class represents a SpaceNav motion event.
 */
public class SpaceNavMotionEvent extends SpaceNavEvent {

  /** translatory movement */
  private int x, y, z;
  /** rotational movement */
  private int rx, ry, rz;

  /* private int period; */


  /** Create a new SpaceNavMotionEvent
   * @param x x-translation
   * @param y y-translation
   * @param z z-translation
   * @param rx x-rotation
   * @param ry y-rotation
   * @param rz z-rotation
   */
  public SpaceNavMotionEvent(int x, int y, int z,
      int rx, int ry, int rz
      /*, int period*/) {
	this.x=x;
	this.y=y;
	this.z=z;
	this.rx=rx;
	this.ry=ry;
	this.rz=rz;
	/*this.period=period;*/
  }

  /**
   * Get the X-translation
   * @return the value
   */
  public int getX() {
    return x;
  }

  /**
   * Get the Y-translation
   * @return the value
   */
  public int getY() {
    return y;
  }
  /**
   * Get the Z-translation
   * @return the value
   */
  public int getZ() {
    return z;
  }

  /**
   * Get the X-rotation
   * @return the value
   */
  public int getRX() {
    return rx;
  }

  /**
   * Get the Y-rotation
   * @return the value
   */
  public int getRY() {
    return ry;
  }

  /**
   * Get the Z-rotation
   * @return the value
   */
  public int getRZ() {
    return rz;
  }

  /**
   * Get the period
   * @return the value
   */
  /*
  public int getPeriod() {
    return period;
  }
  */
}
