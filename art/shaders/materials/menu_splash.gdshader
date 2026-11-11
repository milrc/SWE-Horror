shader_type canvas_item;

#include "../includes/hatching.gdshaderinc"

uniform sampler2D ceiling_texture : filter_nearest, repeat_enable;
uniform sampler2D wall_texture : filter_nearest, repeat_enable;
uniform sampler2D floor_texture : filter_nearest, repeat_enable;
uniform bool preview_textures = false;
uniform float boil_speed : hint_range(0, 2) = 0.1;
uniform float warp_speed : hint_range(0, 2) = 0.1;
uniform float warp_strength : hint_range(0, 10) = 0.1;
uniform float walk_speed : hint_range(0, 5) = 0.1;
uniform vec2 uv_scale = vec2(3.0, 1.0);
uniform float boil_strength : hint_range(0, 1) = 0.05;

void fragment() {
    //rotate around the center of the tunnel
    vec2 boil = (0.5 - tunnel_depth(UV))* vec2(sin(TIME*warp_speed), cos(TIME*warp_speed));
    boil *= warp_strength*clamp(1.0 - distance(UV, vec2(0.5)/2.0), 0.0, 1.0); //reduce at the edges
    //boil UV through noise
    boil += vec2(get_noise(UV + fract(TIME*boil_speed)).x, get_noise(UV + fract(TIME*boil_speed + 0.5)).y);

    vec2 screen_uv = UV + boil*boil_strength;
    vec2 tunnel_uv = tunnel_map(screen_uv, vec2(TIME*walk_speed, 0.0), uv_scale);
    float value = 1.0 - tunnel_depth(screen_uv); //darken based on distance
    value *= pow(tunnel_ao(screen_uv), 0.2); //darken corners (increased gamma)
    vec3 color;
    color = texture(wall_texture, tunnel_uv).xyz*(tunnel_left_mask(screen_uv) + tunnel_right_mask(screen_uv)); //draw walls
    color += texture(ceiling_texture, tunnel_uv).xyz*tunnel_top_mask(screen_uv); //draw ceiling
    color += texture(floor_texture, tunnel_uv).xyz*tunnel_bottom_mask(screen_uv); //draw floor
    color *= value; //darken image by value

    //Hatch to appropriate values
    if (!preview_textures) 
      color = hatching_effect(color, screen_uv);

    COLOR = vec4(color, 1.0);
}