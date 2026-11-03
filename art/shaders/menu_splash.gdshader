shader_type canvas_item;

#include "hatching.gdshaderinc"

uniform sampler2D ceiling_texture : filter_nearest, repeat_enable;
uniform sampler2D wall_texture : filter_nearest, repeat_enable;
uniform sampler2D floor_texture : filter_nearest, repeat_enable;
uniform float time_scale : hint_range(0, 5) = 0.1;
uniform float walk_speed : hint_range(0, 5) = 0.1;
uniform vec2 uv_scale = vec2(3.0, 1.0);
uniform float boil_strength : hint_range(0, 1) = 0.05;

void fragment() {
    float time = TIME*time_scale;
    vec2 boil = vec2(get_noise(UV + fract(time)).x, get_noise(UV + fract(time + 0.5)).y);
    vec2 screen_uv = UV + boil*boil_strength;
    vec2 tunnel_uv = tunnel_map(screen_uv, vec2(TIME*walk_speed, 0.0), uv_scale);
    vec3 color;
    float value = 1.0 - tunnel_depth(screen_uv);
    value *= pow(tunnel_ao(screen_uv), 0.2);
    color = texture(wall_texture, tunnel_uv).xyz*(tunnel_left_mask(screen_uv) + tunnel_right_mask(screen_uv)); //walls
    color += texture(ceiling_texture, tunnel_uv).xyz*tunnel_top_mask(screen_uv);
    color += texture(floor_texture, tunnel_uv).xyz*tunnel_bottom_mask(screen_uv);
    color *= value;
    
    //Hatch to appropriate values
    color = hatching_effect(color, screen_uv);

    COLOR = vec4(color, 1.0);
}