using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// Пространство имён для обработки MSIL кода.
/// </summary>
namespace BogdanovUtilitisLib.MSILHandle
{
    /// <summary>
    /// Класс для обработки MSIL кода.
    /// </summary>
    public class MSILHandler
    {
        #region Private values and properties

        static Dictionary<short, OpCode> _opcodes = new Dictionary<short, OpCode>();
        StringBuilder _output;
        Module _module;
        byte[] _il;
        int _pos;

        #endregion

        #region Public

        /// <summary>
        /// Получение дизасемблированного кода.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string Disassemble(MethodBase method)
    => new MSILHandler(method).Dis();

        #endregion

        #region Private

        static MSILHandler()
        {
            Dictionary<short, OpCode> opcodes = new Dictionary<short, OpCode>();
            foreach (FieldInfo fi in typeof(OpCodes)
                .GetFields(BindingFlags.Public |
                BindingFlags.Static))
            {
                if (typeof(OpCode).IsAssignableFrom(fi.FieldType))
                {
                    OpCode code = (OpCode)fi.GetValue(null);
                    if (code.OpCodeType != OpCodeType.Nternal)
                    {
                        _opcodes.Add(code.Value, code);
                    }
                }
            }
        }

        MSILHandler(System.Reflection.MethodBase method)
        {
            _module = method.DeclaringType.Module;
            _il = method.GetMethodBody().GetILAsByteArray();
        }

        string Dis()
        {
            _output = new StringBuilder();
            while (_pos < _il.Length)
            {
                DissasembleNextInstraction();
            }
            return _output.ToString();
        }

        private void DissasembleNextInstraction()
        {
            int opStart = _pos;
            OpCode code = ReadOpCode();
            string operand = ReadOperand(code);
            _output.AppendFormat("IL_{0:X4}: {1, -12} {2}",
                opStart, code.Name, operand);
            _output.AppendLine();
        }

        private OpCode ReadOpCode()
        {
            byte byteCode = _il[_pos++];
            if (_opcodes.ContainsKey(byteCode)) return _opcodes[byteCode];
            if (_pos == _il.Length) throw new Exception("Неожиданный конец кода");
            short shortCode = (short)(byteCode * 256 + _il[_pos++]);
            if (!_opcodes.ContainsKey(shortCode))
            {
                throw new Exception("Не найден опкод" + shortCode);
            }
            return _opcodes[shortCode];
        }

        private string ReadOperand(OpCode c)
        {
            int operandLength = c.OperandType == OperandType.InlineNone ? 0 :
                c.OperandType == OperandType.ShortInlineBrTarget ||
                c.OperandType == OperandType.ShortInlineI ||
                c.OperandType == OperandType.ShortInlineVar ? 1 :
                c.OperandType == OperandType.InlineVar ? 2 :
                c.OperandType == OperandType.InlineI8 ||
                c.OperandType == OperandType.InlineR ? 8 :
                c.OperandType == OperandType.InlineSwitch ?
                4 * (BitConverter.ToInt32(_il, _pos) + 1) : 4;
            if (_pos + operandLength > _il.Length) throw new Exception("Неожиданный конец кода");
            string result = FormatOperand(c, operandLength);
            if (result == null)
            {
                result = "";
                for (int i = 0; i < operandLength; i++)
                    result += _il[_pos + i].ToString("X2") + " ";
            }
            _pos += operandLength;
            return result;
        }

        private string FormatOperand(OpCode c, int operandLength)
        {
            if (operandLength == 0) return "";

            if (operandLength == 4) return Get4ByteOperand(c);
            else if (c.OperandType == OperandType.ShortInlineBrTarget) return GetShortRelativeTarget();
            else if (c.OperandType == OperandType.InlineSwitch) return GetSwitchTarget(operandLength);
            else return null;
        }

        private string Get4ByteOperand(OpCode c)
        {
            int intOp = BitConverter.ToInt32(_il, _pos);
            switch (c.OperandType)
            {
                case OperandType.InlineBrTarget:
                    return "IL_" + (_pos + intOp + 4).ToString("X4");
                case OperandType.InlineString:
                    string s = _module.ResolveString(intOp);
                    if (s != null) s = "'" + s + "'";
                    return s;
                case OperandType.InlineField:
                case OperandType.InlineMethod:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                    System.Reflection.MemberInfo mi;
                    try
                    {
                        mi = _module.ResolveMember(intOp);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    if (mi == null) return null;
                    if (mi.ReflectedType != null) return mi.ReflectedType.FullName + "." + mi.Name;
                    else if (mi is Type) return ((Type)mi).FullName;
                    else return mi.Name;
                default:
                    return null;
            }
        }

        private string GetShortRelativeTarget()
        {
            int absoluteTarget = _pos + (sbyte)_il[_pos] + 1;
            return "IL_" + absoluteTarget.ToString("X4");
        }

        private string GetSwitchTarget(int operandLength)
        {
            int targetCount = BitConverter.ToInt32(_il, _pos);
            string[] targets = new string[targetCount];
            for (int i = 0; i < targetCount; i++)
            {
                int ilTarget = BitConverter.ToInt32(_il, _pos + (i + 1) * 4);
                targets[i] = "IL_" + (_pos + ilTarget + operandLength).ToString("X4");
            }
            return "(" + string.Join(", ", targets) + ")";
        }

        #endregion
    }
}
