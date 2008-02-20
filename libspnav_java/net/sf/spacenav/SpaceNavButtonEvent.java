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
 * This class represents a SpaceNav button event.
 */
public class SpaceNavButtonEvent extends SpaceNavEvent {

  /** was the button pressed (true) or released (false)? */
  private boolean pressed;

  /** the number of the button */
  private int button;


  /**
   * Create a new SpaceNavButtonEvent
   * @param button button-number
   * @param pressed pressed (true) or released (false)
   */
  public SpaceNavButtonEvent(int button, boolean pressed) {
    this.button=button;
    this.pressed=pressed;
  }

  /**
   * Was the button pressed or released?
   * @return true if pressed, false if released
   */
  public boolean isPressed() {
    return pressed;
  }

  /**
   * Get the number of the button.
   * @return the number of the button
   */
  public int getButton() {
    return button;
  }

}
