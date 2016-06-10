﻿//
// MonoBtlsKey.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#if SECURITY_DEP
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Mono.Btls
{
	class MonoBtlsKey : MonoBtlsObject
	{
		internal class BoringKeyHandle : MonoBtlsHandle
		{
			internal BoringKeyHandle ()
				: base ()
			{
			}

			protected override bool ReleaseHandle ()
			{
				mono_btls_key_free (handle);
				return true;
			}

			[DllImport (DLL)]
			extern static void mono_btls_key_free (IntPtr handle);
		}

		[DllImport (DLL)]
		extern static BoringKeyHandle mono_btls_key_up_ref (BoringKeyHandle handle);

		[DllImport (DLL)]
		extern static int mono_btls_key_get_bytes (BoringKeyHandle handle, out IntPtr data, out int size, int include_private_bits);

		[DllImport (DLL)]
		extern static int mono_btls_key_get_bits (BoringKeyHandle handle);

		[DllImport (DLL)]
		extern static int mono_btls_key_is_rsa (BoringKeyHandle handle);

		[DllImport (DLL)]
		extern static int mono_btls_key_test (BoringKeyHandle handle);

		new internal BoringKeyHandle Handle {
			get { return (BoringKeyHandle)base.Handle; }
		}

		internal MonoBtlsKey (BoringKeyHandle handle)
			: base (handle)
		{
		}

		public byte[] GetBytes (bool include_private_bits)
		{
			int size;
			IntPtr data;

			var ret = mono_btls_key_get_bytes (Handle, out data, out size, include_private_bits ? 1 : 0);
			CheckError (ret);

			var buffer = new byte [size];
			Marshal.Copy (data, buffer, 0, size);
			FreeDataPtr (data);
			return buffer;
		}

		public void Test ()
		{
			mono_btls_key_test (Handle);
		}

		public MonoBtlsKey Copy ()
		{
			CheckThrow ();
			var copy = mono_btls_key_up_ref (Handle);
			CheckError (copy != null && !copy.IsInvalid);
			return new MonoBtlsKey (copy);
		}
	}
}
#endif
