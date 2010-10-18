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

#include <string.h>
#include <unistd.h>
#include "serial/serialconstants.h"
#include "serial/serialcommunication.h"
#include "serial/serialmagellan.h"
#include "serial/serialdetect.h"

int detectDevice(const char *devFile, char *buffer, int bufferSize)
{
  int file, bytesRead, tempPosition, length;
  char tempBuffer[MAXREADSIZE];
  tempPosition = 0;
  
  file = openFile(devFile);
  if (file == -1)
    return -1;
  if (setPortSpaceball(file) == -1)
    return -1;
  
  /*first look for spaceball. should have data after open and port setup*/   
  do
  {
     bytesRead = serialRead(file, tempBuffer[tempPosition], MAXREADSIZE-tempPosition);
     tempPosition += bytesRead;
     longWait();
  }while(bytesRead>0);
  if (tempPosition>0)
  {
    if (tempPosition<bufferSize)
    {
      strcpy(tempBuffer, buffer);
      close(file);
      return 0;
    }
  }
  
  /*now if we are here we don't have a spaceball and now we need to check for a magellan*/
  close(file);
  tempPosition = 0;
  file = openFile(devFile);
  if (file == -1)
    return -1;
  if (setPortMagellan(file) == -1)
    return -1;
  get_version_string(file, tempBuffer, VERSION_STRING_MAX);
  length = strlen(tempBuffer);
  if (length < 1)
    return -1;
  if(length < MAXREADSIZE)
  {
    strcpy(tempBuffer, buffer);
    close(file);
    return 0;
  }
  close(file);
  return -1;
}
