/*
serialdetect - detecting serial spaceball/magellan device.
Copyright (C) 2010 Thomas Anderson <ta@nextgenengineering.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#include "serial/serialconstants.h"
#include "serial/serialcommunication.h"
#include "serial/serialdetect.h"

int detectDevice(char *devFile, char *buffer, int bufferSize)
{
  int file, bytesRead;
  if (file = openFile(devFile) == -1)
    return -1;
  if (setPortSpaceball(file) == -1)
    return -1;
  /*first look for spaceball. should have data after open and port setup*/

  do
  {

    
  }
  
  return 0;
}
