import sys
import textwrap
from string import Template

def is_convertible(from_kind, to_kind):
    if from_kind == to_kind:
        return True

    if from_kind == "integer" and to_kind == "number":
        return True

    return False

def mk_indent(num):
    return ' ' * (4 * num)

def main(args):
    template_path = args[1]

    # https://docs.microsoft.com/ja-jp/dotnet/csharp/language-reference/keywords/built-in-types-table
    types = [
        "bool",
        "byte",
        "sbyte",
        "char",
        "decimal",
        "double",
        "float",
        "int",
        "uint",
        "long",
        "ulong",
        "short",
        "ushort",
    ]
    kinds = {
        "bool": "bool",
        "byte": "integer",
        "sbyte": "integer",
        "char": "integer",
        "decimal": "number",
        "double": "number",
        "float": "number",
        "int": "integer",
        "uint": "integer",
        "long": "integer",
        "ulong": "integer",
        "short": "integer",
        "ushort": "integer",
    }

    from_types = ["bool", "long", "double"]

    # Generate convertion tables
    # FromType -> (ToType -> ConversionFunction)
    output_tables = ""
    output_tables += textwrap.indent("""
private static readonly Dictionary<Type, Dictionary<Type, Func<object, object>>> _convTable =
    new Dictionary<Type, Dictionary<Type, Func<object, object>>>
    {
""".lstrip(), mk_indent(2))

    for from_ty in types:
        if from_ty not in from_types:
            continue

        output_tables += textwrap.indent("""
{{
    typeof({0}), new Dictionary<Type, Func<object, object>>
    {{
""".format(from_ty).lstrip(), mk_indent(4))

        for to_ty in types:
            if not is_convertible(kinds[from_ty], kinds[to_ty]):
                continue

            if from_ty != to_ty:
                output_tables += textwrap.indent("""
{{ typeof({1}), o => ConvertFrom{2}To{3}(({0})o) }},
""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()).lstrip(), mk_indent(6))
            else:
                output_tables += textwrap.indent("""
{{ typeof({1}), o => o }},
""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()).lstrip(), mk_indent(6))
            # print(from_ty, to_ty, is_convertible(kinds[from_ty], kinds[to_ty]))

        output_tables += textwrap.indent("""
    }
},
""".lstrip('\n'), mk_indent(4))

    output_tables += textwrap.indent("""
};
""".lstrip(), mk_indent(3))

    # Generate conversion functions
    output_funcs = ""
    for from_ty in types:
        if from_ty not in from_types:
            continue

        for to_ty in types:
            if not is_convertible(kinds[from_ty], kinds[to_ty]):
                continue

            output_funcs += textwrap.indent("""
private static object ConvertFrom{2}To{3}({0} o) {{
    return ({1})o;
}}
""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()), mk_indent(2))

    tmpl = Template(open(template_path).read())
    output = tmpl.substitute(body = output_tables + output_funcs)

    print(output)

if __name__ == "__main__":
    main(sys.argv)
