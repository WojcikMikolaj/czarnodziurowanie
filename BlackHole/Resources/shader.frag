#version 410 core

struct PSInput
{
    vec4 pos;
    vec2 tex;
    vec4 viewVec;
};

in PSInput input;

out vec4 FragColor;

uniform samplerCube cubemap;


//
//vec3 FoldRay(vec3 ray, vec2 coord)
//{
//    
//}

void main()
{
    FragColor = texture(cubemap, input.viewVec.xyz);
    //FragColor = vec4(input.viewVec.xyz, 1);
}