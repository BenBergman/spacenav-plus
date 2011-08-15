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



#include "event.h"

int open_smag(const char *devfile);
int close_smag();
int read_smag(struct dev_input *inp);
int get_fd_smag();

void get_version_string(int fileDescriptor, char *buffer, int buffersize);

void initMagellan(int fileDescriptor);
void clearInput();
void processDisplacementPacket();
void processButtonKPacket();
void processButtonCPacket();
void processButtonNPacket();
void processButtonQPacket();
void readCopy();
void generateDisplacementEvents(int *newValues);
void generateButtonEvent(int button, int newState);
