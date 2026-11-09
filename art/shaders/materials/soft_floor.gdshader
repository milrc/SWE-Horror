shader_type spatial;
render_mode blend_mix, depth_draw_opaque, diffuse_burley, specular_schlick_ggx;

// The regular texture shader looked almost black under the hatch effect.
// Keep some room lighting, but add a little light to the floor itself.
uniform sampler2D texture_albedo : source_color, filter_linear_mipmap;
uniform float floor_brightness : hint_range(0.0, 1.0) = 0.55;

void fragment() {
    vec3 wood = texture(texture_albedo, UV).rgb;
    ALBEDO = wood;
    ROUGHNESS = 0.95;
    EMISSION = wood * floor_brightness;
}
