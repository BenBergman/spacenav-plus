/*
spacenavd - a free software replacement driver for 6dof space-mice.
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

int open_smag();
void close_smag();
void setUpPort();
void longWait();
void shortWait();
void myWriteString(char *string, int count);
int myRead();
int isMagellan();
void initMagellan();
void clearInput();
void processDisplacementPacket();
void processButtonKPacket();
void processButtonCPacket();
void processButtonNPacket();
void processButtonQPacket();
void readCopy();