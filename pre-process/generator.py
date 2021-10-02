import sys
import textwrap
from string import Template

def is_convertible(from_kind_tup, to_kind_tup):
    (from_kind, _) = from_kind_tup
    (to_kind, _) = to_kind_tup

    if from_kind == to_kind:
        return True

    if from_kind == "integer" and to_kind == "number":
        return True

    return False

def is_signed_to_unsigned(from_kind_tup, to_kind_tup):
    (_, from_signed) = from_kind_tup
    (_, to_signed) = to_kind_tup

    if from_signed == "s" and to_signed == "u":
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
        "string",
    ]
    kinds = {
        "bool":    ("bool", "_"),
        "byte":    ("integer", "u"),
        "sbyte":   ("integer", "s"),
        "char":    ("integer", "u"),
        "decimal": ("number", "_"),
        "double":  ("number", "_"),
        "float":   ("number", "_"),
        "int":     ("integer", "s"),
        "uint":    ("integer", "u"),
        "long":    ("integer", "s"),
        "ulong":   ("integer", "u"),
        "short":   ("integer", "s"),
        "ushort":  ("integer", "u"),
        "string":  ("string", "_"),
    }

    from_types = ["bool", "long", "double", "string"]

    # Generate convertion tables
    # FromType -> (ToType -> ConversionFunction)
    output_tables = ""
    output_tables += textwrap.indent("""
private static readonly Dictionary<Type, Dictionary<Type, Converter>> _convTable =
    new Dictionary<Type, Dictionary<Type, Converter>>
    {
""".lstrip(), mk_indent(2))

    for from_ty in types:
        if from_ty not in from_types:
            continue

        output_tables += textwrap.indent("""
{{
    typeof({0}), new Dictionary<Type, Converter>
    {{
""".format(from_ty).lstrip(), mk_indent(4))

        for to_ty in types:
            if not is_convertible(kinds[from_ty], kinds[to_ty]):
                continue

            if from_ty != to_ty:
                output_tables += textwrap.indent("""
{{ typeof({1}), (object i, out object o) => ConvertFrom{2}To{3}(({0})i, out o) }},
""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()).lstrip(), mk_indent(6))
            else:
                output_tables += textwrap.indent("""
{{ typeof({1}), null }},
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

            if from_ty == to_ty:
                continue

            need_signed_check = is_signed_to_unsigned(kinds[from_ty], kinds[to_ty])

            output_funcs += textwrap.indent("""
private static bool ConvertFrom{2}To{3}({0} i, out object o) {{
    try
    {{""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()), mk_indent(2))

            if need_signed_check:
                output_funcs += textwrap.indent("""
        if ( i < 0 )
        {{
            throw new OverflowException();
        }}""".format(), mk_indent(2))

            output_funcs += textwrap.indent("""
        o = checked(({1})i);
        return true;
    }}
    catch(OverflowException)
    {{
        o = null;
        return false;
    }}
}}
""".format(from_ty, to_ty, from_ty.capitalize(), to_ty.capitalize()), mk_indent(2))

    tmpl = Template(open(template_path).read())
    output = tmpl.substitute(body = output_tables + output_funcs)

    print(output)

if __name__ == "__main__":
    main(sys.argv)
