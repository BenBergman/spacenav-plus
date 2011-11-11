/*
serialmagellan - decoding serial magellan spaceball data.
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


#include <stdio.h>
#include <fcntl.h>
#include <termios.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <string.h>
#include "serial/serialconstants.h"
#include "serial/serialevent.h"
#include "serial/serialcommunication.h"
#include "serial/serialmagellan.h"

/*
v  MAGELLAN  Version 6.60  3Dconnexion GmbH 05/11/01
*/									/* magellan spacemouse plus xt serial. after sending vQ\r */


/*
v  MAGELLAN  Version 5.79  by LOGITECH INC. 10/10/97
*/									/* magellan spacemouse. after sending vQ\r */

int file;
struct InputStruct
{
  char readBuf[MAXREADSIZE];
  int readBufSize;
  char packetBuf[MAXPACKETSIZE];
  int packetBufPosition;
  struct event *eventHead;
  struct event *eventTail;
} input;

int firstByteParity[16]={
  0xE0, 0xA0, 0xA0, 0x60, 0xA0, 0x60, 0x60, 0xA0, 0x90, 0x50, 0x50, 0x90, 0xD0, 0x90, 0x90, 0x50
};

int secondByteParity[64]={
  0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80,
  0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0xC0, 0x80, 0x80, 0x40,
  0xC0, 0x80, 0x80, 0x40, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40,
  0x80, 0x40, 0x40, 0x80, 0x40, 0x80, 0x80, 0x40, 0x40, 0x80, 0x80, 0x40, 0x80, 0x40, 0x00, 0x80
};

void clearInput()
{
  memset(input.readBuf, 0x00, MAXREADSIZE);
  input.readBufSize = 0;
  memset(input.packetBuf, 0x00, MAXPACKETSIZE);
  input.packetBufPosition=0;
}

void initMagellan(int fileDescriptor)
{
  serialWriteString(fileDescriptor, "", 0);
  serialWriteString(fileDescriptor, "\r\rm0", 4);
  serialWriteString(fileDescriptor, "pAA", 3);
  serialWriteString(fileDescriptor, "q00", 3);	/*default translation and rotation*/
  serialWriteString(fileDescriptor, "nM", 2);	/*zero radius. 0-15 defaults to 13*/
  serialWriteString(fileDescriptor, "z", 1);	/*zero device*/
  serialWriteString(fileDescriptor, "c33", 3);	/*set translation, rotation on and dominant axis off*/
  serialWriteString(fileDescriptor, "l2\r\0",4);
  serialWriteString(fileDescriptor, "\r\r", 2);
  serialWriteString(fileDescriptor, "l300", 4);
  serialWriteString(fileDescriptor, "b9",2);	/*these are beeps*/
  serialWriteString(fileDescriptor, "b9",2);
  shortWait();
  tcflush(fileDescriptor, TCIOFLUSH);
  clearInput();
}

void readCopy()
{
  int index;
  for (index=0; index<input.readBufSize; ++index)
  {
    if (input.readBuf[index] == '\n' || input.readBuf[index] == '\r')
    {
      input.packetBuf[input.packetBufPosition] = '\0';/*terminate string*/
      
      if (input.packetBuf[0] == 'd' && input.packetBufPosition == 15)
	processDisplacementPacket();
      else if (input.packetBuf[0] == 'k' && input.packetBufPosition == 4)
	processButtonKPacket();
      else if (input.packetBuf[0] == 'c' && input.packetBufPosition == 3)
	processButtonCPacket();
      else if (input.packetBuf[0] == 'n' && input.packetBufPosition == 2)
	processButtonNPacket();
      else if (input.packetBuf[0] == 'q' && input.packetBufPosition == 3)
	processButtonQPacket();
      else
	printf("unknown packet   %s\n",input.packetBuf);
      input.packetBufPosition=0;
    }
    else
    {
      input.packetBuf[input.packetBufPosition] = input.readBuf[index];
      input.packetBufPosition++;
      if (input.packetBufPosition == MAXPACKETSIZE)
      {
	input.packetBufPosition = 0;
	printf("packet buffer overrun\n");
      }
    }
  }
}

int open_smag(const char *devfile)
{
  file = openFile(devfile);
  if (file < 0)
    return -1;
  setPortMagellan(file);
  initMagellan(file);
  clearInput();
  return 0;
}

int close_smag()
{
  serialWriteString(file, "l000", 4);
  close(file);
  return 0;
}

int read_smag(struct dev_input *inp)
{
  /*need to return 1 if we fill in inp or 0 if no events*/
  struct event *currentEvent;
  input.readBufSize = serialRead(file, input.readBuf, MAXREADSIZE);  
  if (input.readBufSize > 0)
    readCopy();  
  currentEvent = input.eventHead;
  if(currentEvent) {
    input.eventHead = input.eventHead->next;
    
    *inp = currentEvent->data;
    free_event(currentEvent);
    return 1;
  }
  return 0;
}

int get_fd_smag()
{
  return file;
}

void get_version_string(int fileDescriptor, char *buffer, int buffersize)
{
  int bytesRead;
  char tempBuffer[MAXREADSIZE];
  serialWriteString(fileDescriptor, "\r\rm0", 4);
  serialWriteString(fileDescriptor, "", 0);
  serialWriteString(fileDescriptor, "\r\rm0", 4);
  serialWriteString(fileDescriptor, "c03", 3);
  serialWriteString(fileDescriptor, "z", 1);
  serialWriteString(fileDescriptor, "Z", 1);
  serialWriteString(fileDescriptor, "l000", 4);
  shortWait();
  tcflush(fileDescriptor, TCIOFLUSH);
  clearInput();
  serialWriteString(fileDescriptor, "vQ", 2);
  bytesRead = serialRead(fileDescriptor, tempBuffer, MAXREADSIZE);
  if (bytesRead> 0 && bytesRead < buffersize)
    strcpy(buffer, tempBuffer);
  clearInput();
}

void generateDisplacementEvents(int *newValues)
{
  int index, pending;
  static int oldValues[6] = {0,0,0,0,0,0};
  struct event *newEvent;
  
  pending = 0;  
  for (index=0;index<6;++index)
  {
    if (newValues[index] == oldValues[index])
      continue;
    oldValues[index] = newValues[index];
    
    newEvent = alloc_event();
    if (newEvent)
    {
      newEvent->data.type = INP_MOTION;
      newEvent->data.idx = index;
      newEvent->data.val = newValues[index];
      newEvent->next = 0;
      
      if(input.eventHead)
      {
	input.eventTail->next = newEvent;
	input.eventTail = newEvent;
      }
      else
	input.eventHead = input.eventTail = newEvent;
      pending = 1;    
    }
  }
    
  if(pending)
  {
    newEvent = alloc_event();
    if(newEvent)
    {
      newEvent->data.type = INP_FLUSH;
      newEvent->next = 0;
    }
    
    if(input.eventHead)
    {
      input.eventTail->next = newEvent;
      input.eventTail = newEvent;
    }
    else
    {
      input.eventHead = input.eventTail = newEvent;
    }
  }  
}

void processDisplacementPacket()
{
  int index, lastBytes, offset, values[6];
  short int accumLast, number, accumLastAdjusted;
  accumLast = offset = 0;
  
  for (index=1; index<13; index+=2)
  {
    /*first byte check*/
    unsigned char low, up;
    low = input.packetBuf[index] & 0x0F;
    up = input.packetBuf[index] & 0xF0;
    if (up != firstByteParity[low])
    {
      printf("bad first packet\n");
      return;
    }
    
    /*second byte check*/
    low = input.packetBuf[index+1] & 0x3F;
    up = input.packetBuf[index+1] & 0xC0;
    if (up != secondByteParity[low])
    {
      printf("bad second packet\n");
      return;
    }
    
    number = 0x0000;
    number = (short int)((input.packetBuf[index] << 6 & 0x03C0) | (input.packetBuf[index+1] & 0x3F));
    if (number>512)
      number = number - 1024;
    accumLast += number;
    
    if (number<0)
    {
      offset--;
      offset += (int) (number+1)/64;
    }
    else
    {
      offset += (int) number/64;
    }
    /*printf("%8i ", number);*/
    values[(index+1)/2-1] = number;
  }
 
  /*last byte of packet is a sum of 6 numbers and a factor of 64. use as a packet check.
  still not sure what the second to last byte is for.*/
  accumLastAdjusted = accumLast & 0x003F;
  accumLastAdjusted += offset;
  if (accumLastAdjusted < 0)
    accumLastAdjusted += 64;
  if (accumLastAdjusted > 63)
    accumLastAdjusted -= 64;
  
  lastBytes = (short int)(input.packetBuf[14] & 0x3F);

  if (accumLastAdjusted != lastBytes)
  {
    printf("   bad packet\n");
    return;
  }
  generateDisplacementEvents(values);
  return;
}

void generateButtonEvent(int button, int newState)
{
  struct event *newEvent = alloc_event();
  if(!newEvent)
    return;
  
  newEvent->data.type = INP_BUTTON;
  newEvent->data.idx = button;
  newEvent->data.val = newState;
  newEvent->next = 0;
  
  if(input.eventHead)
  {
    input.eventTail->next = newEvent;
    input.eventTail = newEvent;
  }
  else
  {
    input.eventHead = input.eventTail = newEvent;
  }  
}

void processButtonKPacket()
{
  static char oldState[5] = {0x00, 0x00, 0x00, 0x00};
  
  if (input.packetBuf[1] != oldState[1])
  {
    if ((input.packetBuf[1] & 0x01) != (oldState[1] & 0x01))
      generateButtonEvent(0, input.packetBuf[1] & 0x01);
    if ((input.packetBuf[1] & 0x02) != (oldState[1] & 0x02))
      generateButtonEvent(1, input.packetBuf[1] & 0x02);
    if ((input.packetBuf[1] & 0x04) != (oldState[1] & 0x04))
      generateButtonEvent(2, input.packetBuf[1] & 0x04);
    if ((input.packetBuf[1] & 0x08) != (oldState[1] & 0x08))
      generateButtonEvent(3, input.packetBuf[1] & 0x08);
  }
  
  if (input.packetBuf[2] != oldState[2])
  {  
    if ((input.packetBuf[2] & 0x01) != (oldState[2] & 0x01))
      generateButtonEvent(4, input.packetBuf[2] & 0x01);
    if ((input.packetBuf[2] & 0x02) != (oldState[2] & 0x02))
      generateButtonEvent(5, input.packetBuf[2] & 0x02);
    if ((input.packetBuf[2] & 0x04) != (oldState[2] & 0x04))
      generateButtonEvent(6, input.packetBuf[2] & 0x04);
    if ((input.packetBuf[2] & 0x08) != (oldState[2] & 0x08))
      generateButtonEvent(7, input.packetBuf[2] & 0x08);
  }
  
  /*skipping asterisk button. asterisk function come in through other packets.*/
  /*magellan plus has left and right (10, 11) buttons not magellan classic */
  /*not sure if we need to filter out lower button events for magellan classic*/
  
  if (input.packetBuf[3] != oldState[3])
  {
    /*
    if (input.packetBuf[3] & 0x01)
      printf("button asterisk   ");
    */
    if ((input.packetBuf[3] & 0x02) != (oldState[3] & 0x02))
      generateButtonEvent(8, input.packetBuf[3] & 0x02);/*left button*/
    if ((input.packetBuf[3] & 0x04) != (oldState[3] & 0x04))
      generateButtonEvent(9, input.packetBuf[3] & 0x04);/*right button*/
  }

  strcpy(oldState, input.packetBuf);
}

void processButtonCPacket()
{
  /*these are implemented at device and these signals are to keep the driver in sync*/
  if (input.packetBuf[1] & 0x02)
    printf("translation is on   ");
  else
    printf("translation is off   ");
  
  if (input.packetBuf[1] & 0x01)
    printf("rotation is on   ");
  else
    printf("rotation is off   ");
  
  if (input.packetBuf[1] & 0x04)
    printf("dominant axis is on   ");
  else
    printf("dominant axis is off   ");
  
  printf("\n");    
  /*printf("%s\n", input.packetBuf);*/
}

void processButtonNPacket()
{
  int radius;
  radius = (int)input.packetBuf[1] & 0x0F;
  printf("zero radius set to %i\n", radius);
}

void processButtonQPacket()
{
  /* this has no effect on the device numbers. Driver is to implement any scale of numbers*/
  int rotation, translation;
  rotation = (int)input.packetBuf[1] & 0x07;
  translation = (int)input.packetBuf[2] & 0x07;
  printf("rotation = %i   translation = %i\n",rotation, translation);
}
