/*{
    "DESCRIPTION": "Generated by Omnidome.",
    "CREDIT": "Generated by Omnidome.",
    "CATEGORIES":
    [
        "Omnidome"
    ],
    "INPUTS":
    [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
 {
 "NAME": "inputmap_mode",
  "LABEL" : "Input mapping",
 "VALUES": [
 2,
 1,
 0
 ],
 "LABELS": [
 "Cubemap",
 "Fisheye",
 "Equirectangular",
 ],
 "DEFAULT": 0,
 "TYPE": "long"
 },
 {
 "NAME": "outputmap_mode",
  "LABEL" : "Output mapping",
 "VALUES": [
 2,
 1,
 0
 ],
 "LABELS": [
 "Cubemap",
 "Fisheye",
 "Equirectangular"
 ],
 "DEFAULT": 0,
 "TYPE": "long"
 },
        {
            "NAME": "map_roll",
            "LABEL" : "Roll",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -180.0,
            "MAX": 180.0
        },
        {
        	"NAME" : "flip",
        	"LABEL" : "Flip",
        	"TYPE" : "bool",
        	"DEFAULT": 0
        },
        {
            "NAME": "map_pitch",
            "LABEL" : "Pitch",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -180.0,
            "MAX": 180.0
        },
        {
            "NAME": "map_yaw",
            "LABEL": "Yaw",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -180.0,
            "MAX": 180.0
        },
        {
            "NAME": "fisheye_stretch",
            "LABEL": "Stretch fisheye",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0
        }
    ]
}
*/
/******************************************************************
    This file is part of Omnidome.

    DomeSimulator is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    DomeSimulator is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with DomeSimulator.  If not, see <http://www.gnu.org/licenses/>.

    Omnidome is free for non-commercial use. If you want to use it
    commercially, you should contact the author
    Michael Winkelmann aka Wilston Oreo by mail:
    me@wilstonoreo.net
**************************************************************************/


#define MAP_EQUIRECTANGULAR 0
#define MAP_FISHEYE 1
#define MAP_CUBEMAP 2

////////// ./shaders/util.h //////////

const float PI = 3.14159265358979323846264;

/// Convert degrees to radians
float deg2rad(in float deg)
{
  return deg * PI / 180.0;
}


/// Convert degrees to radians
float rad2deg(in float rad)
{
  return rad / PI * 180.0;
}

float sqr(in float a)
{
  return a*a;
}

/// Calculates the rotation matrix of a rotation around X axis with an angle in radians
mat3 rotateAroundX( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(1.0,0.0,0.0,
              0.0,  c, -s,
              0.0,  s,  c);
}

/// Calculates the rotation matrix of a rotation around Y axis with an angle in radians
mat3 rotateAroundY( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(  c,0.0,  s,
              0.0,1.0,0.0,
               -s,0.0,  c);
}

/// Calculates the rotation matrix of a rotation around Z axis with an angle in radians
mat3 rotateAroundZ( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(  c, -s,0.0,
                s,  c,0.0,
              0.0,0.0,1.0);
}

/// Calculate rotation by given yaw and pitch angles (in degrees!)
mat3 rotationMatrix(in float yaw, in float pitch, in float roll)
{
  return rotateAroundZ(deg2rad(yaw)) *
         rotateAroundY(deg2rad(pitch)) *
         rotateAroundX(deg2rad(roll));
}

void transform_to_cubemap(inout vec2 texCoords, float offset) {
  float eps =  -0.00;
  texCoords = vec2(texCoords.s/(12.0 - eps) + (0.5 + offset) / 6.0,0.5 + texCoords.t/(2.0 - eps));
}

#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2
#define NO_AXIS 3

int dominant_axis(vec3 uvw) {
    vec3 v = abs(uvw);
    if (v.x >= v.y && (v.x >= v.z)) return X_AXIS;
    if (v.y >= v.x && (v.y >= v.z)) return Y_AXIS;
    if (v.z >= v.x && (v.z >= v.y)) return Z_AXIS;
    return NO_AXIS;
}

float map_equirectangular(in vec3 normal, out vec2 texCoords)
{
    texCoords.s = fract(atan(normal.y,normal.x) / (2.0*PI));
    texCoords.t = fract(acos(normal.z) / PI);
    return 1.0;
}

float map_equirectangular(in vec2 texCoords, out vec3 normal)
{
    float theta = texCoords.t * PI,
    phi = (0.25 - texCoords.s)* 2.0 * PI;
    float sin_theta = sin(theta);
    normal = normalize(vec3(sin_theta * sin(phi), sin_theta * cos(phi), cos(theta)));
    return 1.0;
}


float map_fisheye(in vec3 n, out vec2 texCoords)
{
    vec3 normal = n;
    normal.z = -normal.z;
    float phi = atan(length(normal.xy),normal.z);
    float r = phi / PI * 2.0 / (1.0 + fisheye_stretch);
    if ((r > 1.0) || (r <= 0.0)) return -1.0;
    float theta = atan(n.x,n.y);
    texCoords.s = fract(0.5 * (1.0 + r* cos(theta)));
    texCoords.t = fract(0.5 * (1.0 + r * sin(theta)));
    return 1.0;
}

float map_fisheye(in vec2 texCoords, out vec3 n)
{
    vec2 uv = texCoords -0.5;
    float phi = atan(uv.y,uv.x);
    float l = length(uv);

    if (l > 0.5) return -1.0;

    float theta = l *PI *(1.0 + fisheye_stretch);
    float sin_theta = sin(theta);
    n = normalize(vec3(sin_theta * sin(phi), sin_theta * cos(phi), -cos(theta)));
    return 1.0;
}

#define CUBEMAP_EAST 2.0
#define CUBEMAP_WEST 3.0
#define CUBEMAP_NORTH 0.0
#define CUBEMAP_SOUTH 1.0
#define CUBEMAP_TOP 4.0
#define CUBEMAP_BOTTOM 5.0

float map_cubemap(in vec2 texCoords, out vec3 n) {

  float side = floor(texCoords.x*6.0);
  vec2 uv = (vec2(fract(texCoords.x*6.0),fract(texCoords.y)) - 0.5) *2.0;

  if (side == CUBEMAP_EAST) {
    n = vec3(1.0,uv.x,uv.y);
  } else
  if (side == CUBEMAP_WEST) {
    n = vec3(-1.0,-uv.x,uv.y);
  } else
  if (side == CUBEMAP_NORTH) {
    n = vec3(-uv.x,1.0,uv.y);
  } else
  if (side == CUBEMAP_SOUTH) {
    n = vec3(uv.x,-1.0,uv.y);
  } else
  if (side == CUBEMAP_TOP) {
    n = vec3(uv,1.0);
  } else
  if (side == CUBEMAP_BOTTOM) {
    n = vec3(uv.x,-uv.y,-1.0);
  }

  n = normalize(n);
  return 1.0;
}

float map_cubemap(in vec3 n, out vec2 texCoords) {
  float _off = 0.0;
  vec3 v = abs(n);
  int axis = dominant_axis(n);

  if (axis == X_AXIS) {
    texCoords = n.yz / abs(n.x);
    if (n.x > 0.0) {
        _off = CUBEMAP_EAST;
    } else {
        texCoords.s = -texCoords.s;
        _off = CUBEMAP_WEST;
    }
  }
  if (axis == Y_AXIS)
  {
    texCoords = n.xz / abs(n.y);
    if (n.y > 0.0) {
        _off = CUBEMAP_NORTH;
        texCoords.s = - texCoords.s;
    } else {
        _off = CUBEMAP_SOUTH;
    }
  }

  if (axis == Z_AXIS)
  {
    texCoords = n.xy / abs(n.z);
    if (n.z > 0.0) {
        _off = CUBEMAP_TOP;
    } else {
        _off = CUBEMAP_BOTTOM;
        texCoords.t = - texCoords.t;
    }
  }
  transform_to_cubemap(texCoords,_off);
  return 1.0;
}

void main(void)
{
    vec2 uv=gl_FragCoord.xy/RENDERSIZE.xy;

    if (flip) {
    	uv.y = 1.0 - uv.y;
    }

    vec3 normal = vec3(0.0,0.0,0.0);
    float result = -1.0;

    if (outputmap_mode == MAP_EQUIRECTANGULAR)
    {
        result = map_equirectangular(uv,normal);
    } else
    if (outputmap_mode == MAP_FISHEYE)
    {
        result = map_fisheye(uv,normal);
    } else
    if (outputmap_mode == MAP_CUBEMAP)
    {
        result = map_cubemap(uv,normal);
    }
    if (result < 0.0)
    {
        gl_FragColor = vec4(0.0);
        return;
    }

    normal *= rotationMatrix(map_yaw,map_pitch,map_roll);

    vec2 texCoords;
    if (inputmap_mode == MAP_EQUIRECTANGULAR)
    {
        result = map_equirectangular(normal,uv);
    } else
    if (inputmap_mode == MAP_FISHEYE)
    {
        result = map_fisheye(normal,uv);
    } else
    if (inputmap_mode == MAP_CUBEMAP)
    {
        result = map_cubemap(normal,uv);
    }
    if (result < 0.0)
    {
        gl_FragColor = vec4(0.0);
        return;
    }

    gl_FragColor = texture2DRect(inputImage,uv * _inputImage_imgSize);
}
