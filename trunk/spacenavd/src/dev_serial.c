/*
spacenavd - a free software replacement driver for 6dof space-mice.
Copyright (C) 2007-2010 John Tsiombikas <nuclear@member.fsf.org>

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

#include "stdio.h"
#include "string.h"
#include "dev_serial.h"
#include "serial/serialconstants.h"
#include "serial/sball.h"
#include "serial/serialmagellan.h"
#include "serial/serialdetect.h"

struct Device device;

int open_dev_serial(const char *devfile)
{
        clear_device();

        /*defaulting to libsball functions*/
        strcpy(device.name, "Unknown");
        device.modelId = UNKNOWN;
        device.open_func = sball_open;
        device.close_func = sball_close;
        device.read_func = sball_get_input;
        device.get_fd_func = sball_get_fd;

        if (detectDevice(devfile, device.version_string, VERSION_STRING_MAX) == -1 || device.open_func == 0){
            printf("detectDevice failed\n");
        }
        else{
            if (strlen(device.version_string)>0){
                derive_device_name_model();
                if (device.modelId != UNKNOWN)
                    printf("Found: %s\n", device.name);
                else
                    printf("unknown device for version string: %s\n", device.version_string);
            }
            setup_device();
        }

        if(device.open_func(devfile)==-1) {
            clear_device();
            return -1;
        }
        if (device.get_fd_func)
            return device.get_fd_func();
        else
            return -1;
}

void close_dev_serial()
{
        if (device.close_func != 0)
              device.close_func();
        clear_device();
}

int read_dev_serial(struct dev_input *inp)
{
        if (device.read_func == 0)
            return -1;
        if(!device.read_func(inp)) {
            return -1;
        }
        return 0;
}

void clear_device()
{
        device.name[0]='\0';
        device.version_string[0]='\0';
        device.open_func = 0;
        device.close_func = 0;
        device.read_func = 0;
        device.get_fd_func = 0;
}

void setup_device()
{
  if (device.modelId == MOUSE_PLUS_XT || device.modelId == MOUSE_CLASSIC || device.modelId == MOUSE_PLUS){
      device.open_func = open_smag;
      device.close_func = close_smag;
      device.read_func = read_smag;
      device.get_fd_func = get_fd_smag;
    }
}

void derive_device_name_model()
{
        char *instance;        
        instance = strstr(device.version_string, "MAGELLAN  Version 6.60");
        if (instance){
                strcpy(device.name, "Magellan Plus XT");
                device.modelId = MOUSE_PLUS_XT;
                return;
        }

        instance = strstr(device.version_string, "MAGELLAN  Version 6.70");
        if (instance){
                strcpy(device.name, "Magellan Plus");
                device.modelId = MOUSE_PLUS;
                return;
        }

        instance = strstr(device.version_string, "MAGELLAN  Version 5.79");
        if (instance){
                strcpy(device.name, "Magellan Classic");
                device.modelId = MOUSE_CLASSIC;
                return;
        }
        
        instance = strstr(device.version_string, "Hm2003C");
        if (instance){
                strcpy(device.name, "Spaceball 2003C");
                device.modelId = BALL_2003C;
                return;
        }
        
        instance = strstr(device.version_string, "Hm3003C");
        if (instance){
                strcpy(device.name, "Spaceball 3003C");
                device.modelId = BALL_3003C;
                return;
        }        

        instance = strstr(device.version_string, "Spaceball 4000 FLX");
        if (instance){
                strcpy(device.name, "Spaceball 4000 FLX");
                device.modelId = BALL_4000FLX;
                return;
        }
}

int get_device_id()
{
  return device.modelId;
}
