using System;

[Flags]
public enum INPUT_DISABLE_FACTOR
{
    MAINCITY_ZOOM = 1,
    MAINCITY_VISIBLE = 2,
    MAINCITY_DIRECTOR = 4,
    AREA_SWITCH = 8,
}