
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Text;

namespace QQSDK
{
	public class OpenSSL
	{
		public bool Enabled {get; set;}
		public const string DLLNAME = "libeay32";
		public const string SSLDLLNAME = "ssleay32";
		public enum CryptoLockTypes
		{
			CRYPTO_LOCK_ERR = 1,
			CRYPTO_LOCK_EX_DATA = 2,
			CRYPTO_LOCK_X509 = 3,
			CRYPTO_LOCK_X509_INFO = 4,
			CRYPTO_LOCK_X509_PKEY = 5,
			CRYPTO_LOCK_X509_CRL = 6,
			CRYPTO_LOCK_X509_REQ = 7,
			CRYPTO_LOCK_DSA = 8,
			CRYPTO_LOCK_RSA = 9,
			CRYPTO_LOCK_EVP_PKEY = 10,
			CRYPTO_LOCK_X509_STORE = 11,
			CRYPTO_LOCK_SSL_CTX = 12,
			CRYPTO_LOCK_SSL_CERT = 13,
			CRYPTO_LOCK_SSL_SESSION = 14,
			CRYPTO_LOCK_SSL_SESS_CERT = 15,
			CRYPTO_LOCK_SSL = 16,
			CRYPTO_LOCK_SSL_METHOD = 17,
			CRYPTO_LOCK_RAND = 18,
			CRYPTO_LOCK_RAND2 = 19,
			CRYPTO_LOCK_MALLOC = 20,
			CRYPTO_LOCK_BIO = 21,
			CRYPTO_LOCK_GETHOSTBYNAME = 22,
			CRYPTO_LOCK_GETSERVBYNAME = 23,
			CRYPTO_LOCK_READDIR = 24,
			CRYPTO_LOCK_RSA_BLINDING = 25,
			CRYPTO_LOCK_DH = 26,
			CRYPTO_LOCK_MALLOC2 = 27,
			CRYPTO_LOCK_DSO = 28,
			CRYPTO_LOCK_DYNLOCK = 29,
			CRYPTO_LOCK_ENGINE = 30,
			CRYPTO_LOCK_UI = 31,
			CRYPTO_LOCK_ECDSA = 32,
			CRYPTO_LOCK_EC = 33,
			CRYPTO_LOCK_ECDH = 34,
			CRYPTO_LOCK_BN = 35,
			CRYPTO_LOCK_EC_PRE_COMP = 36,
			CRYPTO_LOCK_STORE = 37,
			CRYPTO_LOCK_COMP = 38,
			CRYPTO_LOCK_FIPS = 39,
			CRYPTO_LOCK_FIPS2 = 40,
			CRYPTO_NUM_LOCKS = 41
		}

#region Delegates

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int err_cb(IntPtr str, uint len, IntPtr u);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int pem_password_cb(IntPtr buf, int size, int rwflag, IntPtr userdata);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GeneratorHandler(int p, int n, IntPtr arg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ObjectNameHandler(IntPtr name, IntPtr arg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void CRYPTO_locking_callback(int mode, int type, string file, int line);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void CRYPTO_id_callback(IntPtr tid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int VerifyCertCallback(int ok, IntPtr x509_store_ctx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int client_cert_cb(IntPtr ssl, out IntPtr x509, out IntPtr pkey);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int alpn_cb(IntPtr ssl, out string selProto, out byte selProtoLen, IntPtr inProtos, int inProtosLen, IntPtr arg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr MallocFunctionPtr(uint num, IntPtr file, int line);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr ReallocFunctionPtr(IntPtr addr, uint num, IntPtr file, int line);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FreeFunctionPtr(IntPtr addr);

#endregion

#region Version

		// 1.0.2a Release
		public const uint Wrapper = 0x1000201F;

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr SSLeay_version(int type);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint SSLeay();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_options();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr MD2_options();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr RC4_options();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr DES_options();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr idea_options();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BF_options();

#endregion

#region Threading

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int CRYPTO_THREADID_set_callback(CRYPTO_id_callback cb);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void CRYPTO_THREADID_set_numeric(IntPtr id, uint val);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void CRYPTO_set_locking_callback(CRYPTO_locking_callback cb);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int CRYPTO_num_locks();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int CRYPTO_add_lock(IntPtr ptr, int amount, CryptoLockTypes type, string file, int line);

#endregion

#region CRYPTO

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void OPENSSL_add_all_algorithms_noconf();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void OPENSSL_add_all_algorithms_conf();

		/// <summary>
		/// #define OPENSSL_malloc(num)	CRYPTO_malloc((int)num,__FILE__,__LINE__)
		/// </summary>
		/// <param name="cbSize"></param>
		/// <returns></returns>
		public static IntPtr OPENSSL_malloc(int cbSize)
		{
			return CRYPTO_malloc(cbSize, System.Reflection.Assembly.GetExecutingAssembly().FullName, 0);
		}

		/// <summary>
		/// #define OPENSSL_free(addr) CRYPTO_free(addr)
		/// </summary>
		/// <param name="p"></param>
		public static void OPENSSL_free(IntPtr p)
		{
			CRYPTO_free(p);
		}

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void CRYPTO_free(IntPtr p);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr CRYPTO_malloc(int num, string file, int line);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int CRYPTO_set_mem_ex_functions(MallocFunctionPtr m, ReallocFunctionPtr r, FreeFunctionPtr f);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void CRYPTO_cleanup_all_ex_data();

#endregion

#region OBJ

		public const int NID_undef = 0;

		public const int OBJ_undef = 0;

		public const int OBJ_NAME_TYPE_UNDEF = 0x0;
		public const int OBJ_NAME_TYPE_MD_METH = 0x1;
		public const int OBJ_NAME_TYPE_CIPHER_METH = 0x2;
		public const int OBJ_NAME_TYPE_PKEY_METH = 0x3;
		public const int OBJ_NAME_TYPE_COMP_METH = 0x4;
		public const int OBJ_NAME_TYPE_NUM = 0x5;

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void OBJ_NAME_do_all(int type, ObjectNameHandler fn, IntPtr arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void OBJ_NAME_do_all_sorted(int type, ObjectNameHandler fn, IntPtr arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int OBJ_txt2nid(string s);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr OBJ_nid2obj(int n);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr OBJ_nid2ln(int n);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr OBJ_nid2sn(int n);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int OBJ_obj2nid(IntPtr o);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr OBJ_txt2obj(string s, int no_name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int OBJ_ln2nid(string s);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int OBJ_sn2nid(string s);

#endregion

#region stack

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_new_null();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int sk_num(IntPtr stack);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int sk_find(IntPtr stack, IntPtr data);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int sk_insert(IntPtr stack, IntPtr data, int where);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_shift(IntPtr stack);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int sk_unshift(IntPtr stack, IntPtr data);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int sk_push(IntPtr stack, IntPtr data);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_pop(IntPtr stack);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_delete(IntPtr stack, int loc);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_delete_ptr(IntPtr stack, IntPtr p);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_value(IntPtr stack, int index);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_set(IntPtr stack, int index, IntPtr data);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr sk_dup(IntPtr stack);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void sk_zero(IntPtr stack);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void sk_free(IntPtr stack);

#endregion

#region SHA

		public const int SHA_DIGEST_LENGTH = 20;

#endregion

#region ASN1

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_INTEGER_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ASN1_INTEGER_free(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_INTEGER_set(IntPtr a, int v);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_INTEGER_get(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_TIME_set(IntPtr s, long t);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_UTCTIME_print(IntPtr bp, IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_TIME_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ASN1_TIME_free(IntPtr x);

		public const int V_ASN1_OCTET_STRING = 4;

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_STRING_type_new(int type);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_STRING_dup(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ASN1_STRING_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_STRING_cmp(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_STRING_set(IntPtr str, byte[] data, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_STRING_data(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_STRING_length(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ASN1_OBJECT_free(IntPtr obj);

#endregion

#region X509_REQ

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_REQ_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_set_version(IntPtr x, int version);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_set_pubkey(IntPtr x, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_REQ_get_pubkey(IntPtr req);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_set_subject_name(IntPtr x, IntPtr name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_sign(IntPtr x, IntPtr pkey, IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_verify(IntPtr x, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_digest(IntPtr data, IntPtr type, byte[] md, ref uint len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_REQ_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_REQ_to_X509(IntPtr r, int days, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_print_ex(IntPtr bp, IntPtr x, uint nmflag, uint cflag);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_REQ_print(IntPtr bp, IntPtr x);

#endregion

#region X509

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_dup(IntPtr x509);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_cmp(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_sign(IntPtr x, IntPtr pkey, IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_check_public_key(IntPtr x509, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_verify(IntPtr x, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_pubkey_digest(IntPtr data, IntPtr type, byte[] md, ref uint len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_digest(IntPtr data, IntPtr type, byte[] md, ref uint len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_version(IntPtr x, int version);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_serialNumber(IntPtr x, IntPtr serial);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_get_serialNumber(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_issuer_name(IntPtr x, IntPtr name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_get_issuer_name(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_subject_name(IntPtr x, IntPtr name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_get_subject_name(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_notBefore(IntPtr x, IntPtr tm);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_notAfter(IntPtr x, IntPtr tm);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_set_pubkey(IntPtr x, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_get_pubkey(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_free(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_verify_cert(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_verify_cert_error_string(int n);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_to_X509_REQ(IntPtr x, IntPtr pkey, IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_print_ex(IntPtr bp, IntPtr x, uint nmflag, uint cflag);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_print(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_find_by_issuer_and_serial(IntPtr sk, IntPtr name, IntPtr serial);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_find_by_subject(IntPtr sk, IntPtr name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_check_trust(IntPtr x, int id, int flags);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_time_adj(IntPtr s, int adj, ref long t);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_gmtime_adj(IntPtr s, int adj);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr d2i_X509_bio(IntPtr bp, ref IntPtr x509);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int i2d_X509_bio(IntPtr bp, IntPtr x509);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_PUBKEY_free(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_OBJECT_up_ref_count(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_OBJECT_free_contents(IntPtr a);

#endregion

#region X509_EXTENSION

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_EXTENSION_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_EXTENSION_free(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_EXTENSION_dup(IntPtr ex);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509V3_EXT_print(IntPtr bio, IntPtr ext, uint flag, int indent);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509V3_EXT_get_nid(int nid);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_add_ext(IntPtr x, IntPtr ex, int loc);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_add1_ext_i2d(IntPtr x, int nid, byte[] value, int crit, uint flags);

		//X509_EXTENSION* X509V3_EXT_conf_nid(LHASH* conf, X509V3_CTX* ctx, int ext_nid, char* value);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509V3_EXT_conf_nid(IntPtr conf, IntPtr ctx, int ext_nid, string value);

		//X509_EXTENSION* X509_EXTENSION_create_by_NID(X509_EXTENSION** ex, int nid, int crit, ASN1_OCTET_STRING* data);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_EXTENSION_create_by_NID(IntPtr ex, int nid, int crit, IntPtr data);

		//X509_EXTENSION* X509_EXTENSION_create_by_OBJ(X509_EXTENSION** ex, ASN1_OBJECT* obj, int crit, ASN1_OCTET_STRING* data);
		//int X509_EXTENSION_set_object(X509_EXTENSION* ex, ASN1_OBJECT* obj);
		//int X509_EXTENSION_set_critical(X509_EXTENSION* ex, int crit);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_EXTENSION_set_critical(IntPtr ex, int crit);

		//int X509_EXTENSION_set_data(X509_EXTENSION* ex, ASN1_OCTET_STRING* data);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_EXTENSION_set_data(IntPtr ex, IntPtr data);

		//ASN1_OBJECT* X509_EXTENSION_get_object(X509_EXTENSION* ex);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_EXTENSION_get_object(IntPtr ex);

		//ASN1_OCTET_STRING* X509_EXTENSION_get_data(X509_EXTENSION* ne);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_EXTENSION_get_data(IntPtr ne);

		//int X509_EXTENSION_get_critical(X509_EXTENSION* ex);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_EXTENSION_get_critical(IntPtr ex);

#endregion

#region X509_STORE

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_STORE_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_STORE_add_cert(IntPtr ctx, IntPtr x);

		//[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		//void X509_STORE_set_flags();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_STORE_free(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_STORE_up_ref(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_STORE_CTX_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_STORE_CTX_init(IntPtr ctx, IntPtr store, IntPtr x509, IntPtr chain);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_STORE_CTX_free(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_STORE_CTX_get_current_cert(IntPtr x509_store_ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_STORE_CTX_get_error_depth(IntPtr x509_store_ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_STORE_CTX_get0_store(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_STORE_CTX_get_error(IntPtr x509_store_ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_STORE_CTX_set_error(IntPtr x509_store_ctx, int error);

#endregion

#region X509_INFO

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_INFO_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_INFO_up_ref(IntPtr a);

#endregion

#region X509_NAME

		public const int MBSTRING_FLAG = 0x1000;

		public const int MBSTRING_ASC = MBSTRING_FLAG | 1;

		public const int ASN1_STRFLGS_RFC2253 = ASN1_STRFLGS_ESC_2253 | ASN1_STRFLGS_ESC_CTRL | ASN1_STRFLGS_ESC_MSB | ASN1_STRFLGS_UTF8_CONVERT | ASN1_STRFLGS_DUMP_UNKNOWN | ASN1_STRFLGS_DUMP_DER;

		public const int ASN1_STRFLGS_ESC_2253 = 1;
		public const int ASN1_STRFLGS_ESC_CTRL = 2;
		public const int ASN1_STRFLGS_ESC_MSB = 4;
		public const int ASN1_STRFLGS_ESC_QUOTE = 8;
		public const int ASN1_STRFLGS_UTF8_CONVERT = 0x10;
		public const int ASN1_STRFLGS_DUMP_UNKNOWN = 0x100;
		public const int ASN1_STRFLGS_DUMP_DER = 0x200;
		public const int XN_FLAG_SEP_COMMA_PLUS = (1 << 16);
		public const int XN_FLAG_FN_SN = 0;

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_NAME_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void X509_NAME_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_NAME_dup(IntPtr xn);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_cmp(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_entry_count(IntPtr name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_add_entry_by_NID(IntPtr name, int nid, int type, byte[] bytes, int len, int loc, int set);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_add_entry_by_txt(IntPtr name, byte[] field, int type, byte[] bytes, int len, int loc, int set);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_get_text_by_NID(IntPtr name, int nid, byte[] buf, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_NAME_get_entry(IntPtr name, int loc);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_NAME_delete_entry(IntPtr name, int loc);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_get_index_by_NID(IntPtr name, int nid, int lastpos);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_digest(IntPtr data, IntPtr type, byte[] md, ref uint len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr X509_NAME_oneline(IntPtr a, byte[] buf, int size);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_print(IntPtr bp, IntPtr name, int obase);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int X509_NAME_print_ex(IntPtr bp, IntPtr nm, int indent, uint flags);

#endregion

#region RAND

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_set_rand_method(IntPtr meth);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr RAND_get_rand_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void RAND_cleanup();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void RAND_seed(byte[] buf, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_pseudo_bytes(byte[] buf, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_bytes(byte[] buf, int num);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void RAND_add(byte[] buf, int num, double entropy);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_load_file(string file, int max_bytes);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_write_file(string file);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static string RAND_file_name(byte[] buf, uint num);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_status();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_query_egd_bytes(string path, byte[] buf, int bytes);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_egd(string path);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_egd_bytes(string path, int bytes);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RAND_poll();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_rand(IntPtr rnd, int bits, int top, int bottom);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_pseudo_rand(IntPtr rnd, int bits, int top, int bottom);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_rand_range(IntPtr rnd, IntPtr range);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_pseudo_rand_range(IntPtr rnd, IntPtr range);

#endregion

#region DSA

		//[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		//public extern static IntPtr DSA_generate_parameters(int bits, byte[] seed, int seed_len, IntPtr counter_ret, IntPtr h_ret, IntPtr callback, IntPtr cb_arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_generate_parameters_ex(IntPtr dsa, int bits, byte[] seed, int seed_len, out int counter_ret, out IntPtr h_ret, bn_gencb_st callback);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_generate_key(IntPtr dsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr DSA_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void DSA_free(IntPtr dsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_up_ref(IntPtr dsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_size(IntPtr dsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSAparams_print(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_print(IntPtr bp, IntPtr x, int off);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_sign(int type, byte[] dgst, int dlen, byte[] sig, out uint siglen, IntPtr dsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DSA_verify(int type, byte[] dgst, int dgst_len, byte[] sigbuf, int siglen, IntPtr dsa);

#endregion

#region RSA

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr RSA_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void RSA_free(IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_up_ref(IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_size(IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_generate_key_ex(IntPtr rsa, int bits, IntPtr e, bn_gencb_st cb);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_check_key(IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_public_encrypt(int flen, byte[] from, byte[] to, IntPtr rsa, int padding);



		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_public_decrypt(int flen, byte[] from, byte[] to, IntPtr rsa, int padding);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_sign(int type, byte[] m, uint m_length, byte[] sigret, out uint siglen, IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_verify(int type, byte[] m, uint m_length, byte[] sigbuf, uint siglen, IntPtr rsa);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int RSA_print(IntPtr bp, IntPtr r, int offset);

#endregion

#region DH

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr DH_generate_parameters(int prime_len, int generator, IntPtr callback, IntPtr cb_arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_generate_parameters_ex(IntPtr dh, int prime_len, int generator, bn_gencb_st cb);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_generate_key(IntPtr dh);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_compute_key(byte[] key, IntPtr pub_key, IntPtr dh);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr DH_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void DH_free(IntPtr dh);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_up_ref(IntPtr dh);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_check(IntPtr dh, out int codes);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DHparams_print(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int DH_size(IntPtr dh);

#endregion

#region BIGNUM

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_value_one();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_CTX_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_CTX_init(IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_CTX_free(IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_CTX_start(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_CTX_get(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_CTX_end(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_init(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_bin2bn(byte[] s, int len, IntPtr ret);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_bn2bin(IntPtr a, byte[] to);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_clear_free(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_clear(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_dup(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_copy(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void BN_swap(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_cmp(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_sub(IntPtr r, IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_add(IntPtr r, IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_mul(IntPtr r, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_num_bits(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_sqr(IntPtr r, IntPtr a, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_div(IntPtr dv, IntPtr rem, IntPtr m, IntPtr d, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_print(IntPtr fp, IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_bn2hex(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_bn2dec(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_hex2bn(out IntPtr a, byte[] str);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_dec2bn(out IntPtr a, byte[] str);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint BN_mod_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint BN_div_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_mul_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_add_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_sub_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int BN_set_word(IntPtr a, uint w);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint BN_get_word(IntPtr a);
		//#define BN_GENCB_set(gencb, callback, cb_arg) { \
		//        BN_GENCB *tmp_gencb = (gencb); \
		//        tmp_gencb->ver = 2; \
		//        tmp_gencb->arg = (cb_arg); \
		//        tmp_gencb->cb.cb_2 = (callback); }

		[StructLayout(LayoutKind.Sequential)]
		public class bn_gencb_st
		{
			/// To handle binary (in)compatibility 
			public uint ver;
			/// callback-specific data 
			public IntPtr arg;
			public GeneratorHandler cb;
		}

#endregion

#region DER


		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr d2i_DHparams(out IntPtr a, IntPtr pp, int length);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int i2d_DHparams(IntPtr a, IntPtr pp);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ASN1_d2i_bio(IntPtr xnew, IntPtr d2i, IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ASN1_i2d_bio(IntPtr i2d, IntPtr bp, IntPtr x);

#endregion

#region PEM

#region X509

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_PKCS7(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr d2i_PKCS7_bio(IntPtr bp, IntPtr p7);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void PKCS7_free(IntPtr p7);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr d2i_PKCS12_bio(IntPtr bp, IntPtr p12);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int i2d_PKCS12_bio(IntPtr bp, IntPtr p12);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PKCS12_create(string pass, string name, IntPtr pkey, IntPtr cert, IntPtr ca, int nid_key, int nid_cert, int iter, int mac_iter, int keytype);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PKCS12_parse(IntPtr p12, string pass, out IntPtr pkey, out IntPtr cert, out IntPtr ca);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void PKCS12_free(IntPtr p12);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_PKCS8publicKey(IntPtr bp, IntPtr evp_pkey, IntPtr evp_cipher, IntPtr kstr, int klen, pem_password_cb cb, IntPtr user_data);

#endregion

#region X509_INFO

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509_INFO(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509_INFO(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region X509_AUX

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509_AUX(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509_AUX(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region X509_REQ

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509_REQ(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509_REQ(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region X509_REQ_NEW

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509_REQ_NEW(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509_REQ_NEW(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region X509_CRL

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_X509_CRL(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_X509_CRL(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region X509Chain

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_X509_INFO_read_bio(IntPtr bp, IntPtr sk, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_X509_INFO_write_bio(IntPtr bp, IntPtr xi, IntPtr enc, byte[] kstr, int klen, IntPtr cd, IntPtr u);

#endregion

#region DSA

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_DSApublicKey(IntPtr bp, IntPtr x, IntPtr enc, byte[] kstr, int klen, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_DSApublicKey(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_DSA_PUBKEY(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_DSA_PUBKEY(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region DSAparams

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_DSAparams(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_DSAparams(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region RSA

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_RSA_PUBKEY(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_RSA_PUBKEY(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_RSApublicKey(IntPtr bp, IntPtr x, IntPtr enc, byte[] kstr, int klen, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_RSApublicKey(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region DHparams

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_DHparams(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_DHparams(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region publicKey

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_publicKey(IntPtr bp, IntPtr x, IntPtr enc, byte[] kstr, int klen, pem_password_cb cb, IntPtr u);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_publicKey(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#region PUBKEY

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int PEM_write_bio_PUBKEY(IntPtr bp, IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr PEM_read_bio_PUBKEY(IntPtr bp, IntPtr x, pem_password_cb cb, IntPtr u);

#endregion

#endregion

#region Constants

		public const int EVP_MAX_MD_SIZE = 64;
		//!!(16+20);
		public const int EVP_MAX_KEY_LENGTH = 32;
		public const int EVP_MAX_IV_LENGTH = 16;
		public const int EVP_MAX_BLOCK_LENGTH = 32;

		public const int EVP_CIPH_STREAM_CIPHER = 0x0;
		public const int EVP_CIPH_ECB_MODE = 0x1;
		public const int EVP_CIPH_CBC_MODE = 0x2;
		public const int EVP_CIPH_CFB_MODE = 0x3;
		public const int EVP_CIPH_OFB_MODE = 0x4;
		public const int EVP_CIPH_MODE = 0x7;
		public const int EVP_CIPH_VARIABLE_LENGTH = 0x8;
		public const int EVP_CIPH_CUSTOM_IV = 0x10;
		public const int EVP_CIPH_ALWAYS_CALL_INIT = 0x20;
		public const int EVP_CIPH_CTRL_INIT = 0x40;
		public const int EVP_CIPH_CUSTOM_KEY_LENGTH = 0x80;
		public const int EVP_CIPH_NO_PADDING = 0x100;
		public const int EVP_CIPH_FLAG_FIPS = 0x400;
		public const int EVP_CIPH_FLAG_NON_FIPS_ALLOW = 0x800;

#endregion

#region Message Digests

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_md_null();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_md2();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_md4();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_md5();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha224();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha256();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha384();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_sha512();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_dss();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_dss1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_mdc2();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_ripemd160();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_ecdsa();

#endregion

#region HMAC

		public const int HMAC_MAX_MD_CBLOCK = 128;

		//!!void HMAC_CTX_init(HMAC_CTX *ctx);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_CTX_init(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_CTX_set_flags(IntPtr ctx, uint flags);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_CTX_cleanup(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_Init(IntPtr ctx, byte[] key, int len, IntPtr md);
		// deprecated 

		//!!public extern static void HMAC_Init_ex(IntPtr ctx, const void *key, int len, const EVP_MD *md, ENGINE *impl);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_Init_ex(IntPtr ctx, byte[] key, int len, IntPtr md, IntPtr engine_impl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_Update(IntPtr ctx, byte[] data, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void HMAC_Final(IntPtr ctx, byte[] md, ref uint len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr HMAC(IntPtr evp_md, byte[] key, int key_len, byte[] d, int n, byte[] md, ref uint md_len);

#endregion

#region Ciphers

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_get_cipherbyname(byte[] name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_enc_null();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_cfb1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_cfb8();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_cfb1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_cfb8();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_des_ede3_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_desx_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc4();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc4_40();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_idea_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_idea_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_idea_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_idea_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_40_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_64_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc2_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_bf_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_bf_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_bf_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_bf_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_cast5_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_cast5_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_cast5_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_cast5_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc5_32_12_16_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc5_32_12_16_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc5_32_12_16_cfb64();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_rc5_32_12_16_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_cfb1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_cfb8();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_cfb128();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_128_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_cfb1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_cfb8();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_cfb128();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_192_ofb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_ecb();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_cbc();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_cfb1();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_cfb8();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_cfb128();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_aes_256_ofb();

#endregion

#region EVP_PKEY

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_PKEY_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EVP_PKEY_free(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_cmp(IntPtr a, IntPtr b);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_decrypt(byte[] dec_key, byte[] enc_key, int enc_key_len, IntPtr public_key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_encrypt(byte[] enc_key, byte[] key, int key_len, IntPtr pub_key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_encrypt_old(byte[] enc_key, byte[] key, int key_len, IntPtr pub_key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_type(int type);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_bits(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_size(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_assign(IntPtr pkey, int type, IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_set1_DSA(IntPtr pkey, IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_PKEY_get1_DSA(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_set1_RSA(IntPtr pkey, IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_PKEY_get1_RSA(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_set1_EC_KEY(IntPtr pkey, IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_PKEY_get1_EC_KEY(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_set1_DH(IntPtr pkey, IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_PKEY_get1_DH(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_copy_parameters(IntPtr to, IntPtr from);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_missing_parameters(IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_save_parameters(IntPtr pkey, int mode);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_PKEY_cmp_parameters(IntPtr a, IntPtr b);

#endregion

#region EVP_CIPHER

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EVP_CIPHER_CTX_init(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_CTX_rand_key(IntPtr ctx, byte[] key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_CTX_set_padding(IntPtr x, int padding);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_CTX_set_key_length(IntPtr x, int keylen);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_CTX_ctrl(IntPtr ctx, int type, int arg, IntPtr ptr);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_CTX_cleanup(IntPtr a);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CIPHER_type(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CipherInit_ex(IntPtr ctx, IntPtr type, IntPtr impl, byte[] key, byte[] iv, int enc);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CipherUpdate(IntPtr ctx, byte[] outb, out int outl, byte[] inb, int inl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_CipherFinal_ex(IntPtr ctx, byte[] outm, ref int outl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_OpenInit(IntPtr ctx, IntPtr type, byte[] ek, int ekl, byte[] iv, IntPtr priv);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_OpenFinal(IntPtr ctx, byte[] outb, out int outl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_SealInit(IntPtr ctx, IntPtr type, IntPtr[] ek, int[] ekl, byte[] iv, IntPtr[] pubk, int npubk);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_SealFinal(IntPtr ctx, byte[] outb, out int outl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_DecryptUpdate(IntPtr ctx, byte[] output, out int outl, byte[] input, int inl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_EncryptInit_ex(IntPtr ctx, IntPtr cipher, IntPtr impl, byte[] key, byte[] iv);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_EncryptUpdate(IntPtr ctx, byte[] output, out int outl, byte[] input, int inl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_BytesToKey(IntPtr type, IntPtr md, byte[] salt, byte[] data, int datal, int count, byte[] key, byte[] iv);

#endregion

#region EVP_MD

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_MD_type(IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_MD_pkey_type(IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_MD_size(IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_MD_block_size(IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint EVP_MD_flags(IntPtr md);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_get_digestbyname(byte[] name);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EVP_MD_CTX_init(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_MD_CTX_cleanup(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EVP_MD_CTX_create();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EVP_MD_CTX_destroy(IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_DigestInit_ex(IntPtr ctx, IntPtr type, IntPtr impl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_DigestUpdate(IntPtr ctx, byte[] d, uint cnt);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_DigestFinal_ex(IntPtr ctx, byte[] md, ref uint s);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_Digest(byte[] data, uint count, byte[] md, ref uint size, IntPtr type, IntPtr impl);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_SignFinal(IntPtr ctx, byte[] md, ref uint s, IntPtr pkey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EVP_VerifyFinal(IntPtr ctx, byte[] sigbuf, uint siglen, IntPtr pkey);

#endregion

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_get_builtin_curves(IntPtr r, int nitems);

#region EC_METHOD

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GFp_simple_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GFp_mont_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GFp_nist_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GF2m_simple_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_METHOD_get_field_type(IntPtr meth);

#endregion

#region EC_GROUP

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_new(IntPtr meth);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_GROUP_free(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_GROUP_clear_free(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_copy(IntPtr dst, IntPtr src);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_dup(IntPtr src);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_method_of(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_set_generator(IntPtr group, IntPtr generator, IntPtr order, IntPtr cofactor);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_get0_generator(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_order(IntPtr group, IntPtr order, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_cofactor(IntPtr group, IntPtr cofactor, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_GROUP_set_curve_name(IntPtr group, int nid);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_curve_name(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_GROUP_set_asn1_flag(IntPtr group, int flag);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_asn1_flag(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_GROUP_set_point_conversion_form(IntPtr x, int y);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_point_conversion_form(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static byte[] EC_GROUP_get0_seed(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_seed_len(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_set_seed(IntPtr x, byte[] buf, int len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_set_curve_GFp(IntPtr group, IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_curve_GFp(IntPtr group, IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_set_curve_GF2m(IntPtr group, IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_curve_GF2m(IntPtr group, IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_get_degree(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_check(IntPtr group, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_check_discriminant(IntPtr group, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_cmp(IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_new_curve_GFp(IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_new_curve_GF2m(IntPtr p, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_GROUP_new_by_curve_name(int nid);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_precompute_mult(IntPtr group, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_GROUP_have_precompute_mult(IntPtr group);

#endregion

#region EC_POINT

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_new(IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_POINT_free(IntPtr point);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_POINT_clear_free(IntPtr point);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_copy(IntPtr dst, IntPtr src);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_dup(IntPtr src, IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_method_of(IntPtr point);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_to_infinity(IntPtr group, IntPtr point);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_Jprojective_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr z, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_get_Jprojective_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr z, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_affine_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_get_affine_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_compressed_coordinates_GFp(IntPtr group, IntPtr p, IntPtr x, int y_bit, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_affine_coordinates_GF2m(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_get_affine_coordinates_GF2m(IntPtr group, IntPtr p, IntPtr x, IntPtr y, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_set_compressed_coordinates_GF2m(IntPtr group, IntPtr p, IntPtr x, int y_bit, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_point2oct(IntPtr group, IntPtr p, int form, byte[] buf, int len, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_oct2point(IntPtr group, IntPtr p, byte[] buf, int len, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_point2bn(IntPtr a, IntPtr b, int form, IntPtr c, IntPtr d);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_bn2point(IntPtr a, IntPtr b, IntPtr c, IntPtr d);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static string EC_POINT_point2hex(IntPtr a, IntPtr b, int form, IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_POINT_hex2point(IntPtr a, string s, IntPtr b, IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_add(IntPtr group, IntPtr r, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_dbl(IntPtr group, IntPtr r, IntPtr a, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_invert(IntPtr group, IntPtr a, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_is_at_infinity(IntPtr group, IntPtr p);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_is_on_curve(IntPtr group, IntPtr point, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_cmp(IntPtr group, IntPtr a, IntPtr b, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_make_affine(IntPtr a, IntPtr b, IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINTs_make_affine(IntPtr a, int num, IntPtr[] b, IntPtr c);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINTs_mul(IntPtr group, IntPtr r, IntPtr n, int num, IntPtr[] p, IntPtr[] m, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_POINT_mul(IntPtr group, IntPtr r, IntPtr n, IntPtr q, IntPtr m, IntPtr ctx);

#endregion

#region EC_KEY

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr EC_KEY_dup_func(IntPtr x);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void EC_KEY_free_func(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_new_by_curve_name(int nid);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_KEY_free(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_copy(IntPtr dst, IntPtr src);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_dup(IntPtr src);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_up_ref(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_get0_group(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_set_group(IntPtr key, IntPtr group);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_get0_private_key(IntPtr key);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_bn2mpi(IntPtr key, byte[] PrivateKey);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_set_private_key(IntPtr eckey, IntPtr BN);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr BN_mpi2bn(byte[] PrivateKey, int PrivateKeyLen, IntPtr BN);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_get0_public_key(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_set_public_key(IntPtr key, IntPtr pub);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static uint EC_KEY_get_enc_flags(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_KEY_set_enc_flags(IntPtr x, uint y);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_get_conv_form(IntPtr x);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_KEY_set_conv_form(IntPtr x, int y);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr EC_KEY_get_key_method_data(IntPtr x, EC_KEY_dup_func dup_func, EC_KEY_free_func free_func, EC_KEY_free_func clear_free_func);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_KEY_insert_key_method_data(IntPtr x, IntPtr data, EC_KEY_dup_func dup_func, EC_KEY_free_func free_func, EC_KEY_free_func clear_free_func);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void EC_KEY_set_asn1_flag(IntPtr x, int y);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_precompute_mult(IntPtr key, IntPtr ctx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_generate_key(IntPtr key);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int EC_KEY_check_key(IntPtr key);

#endregion

#region ECDSA

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_SIG_new();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ECDSA_SIG_free(IntPtr sig);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int i2d_ECDSA_SIG(IntPtr sig, byte[] pp);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr d2i_ECDSA_SIG(IntPtr sig, byte[] pp, long len);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_do_sign(byte[] dgst, int dgst_len, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_do_sign_ex(byte[] dgst, int dgstlen, IntPtr kinv, IntPtr rp, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_do_verify(byte[] dgst, int dgst_len, IntPtr sig, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_OpenSSL();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ECDSA_set_default_method(IntPtr meth);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_get_default_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_set_method(IntPtr eckey, IntPtr meth);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_size(IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_sign_setup(IntPtr eckey, IntPtr ctx, IntPtr kinv, IntPtr rp);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_sign(int type, byte[] dgst, int dgstlen, byte[] sig, ref uint siglen, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_sign_ex(int type, byte[] dgst, int dgstlen, byte[] sig, ref uint siglen, IntPtr kinv, IntPtr rp, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_verify(int type, byte[] dgst, int dgstlen, byte[] sig, int siglen, IntPtr eckey);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_get_ex_new_index(IntPtr argl, IntPtr argp, IntPtr new_func, IntPtr dup_func, IntPtr free_func);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDSA_set_ex_data(IntPtr d, int idx, IntPtr arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDSA_get_ex_data(IntPtr d, int idx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ERR_load_ECDSA_strings();

#endregion

#region ECDH

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDH_OpenSSL();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ECDH_set_default_method(IntPtr method);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDH_get_default_method();

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDH_set_method(IntPtr key, IntPtr method);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr ECDH_KDF([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pin, int inlen, IntPtr pout, ref int outlen);

		//<DllImport(DLLNAME, CallingConvention:=CallingConvention.Cdecl)>
		//Public Shared Function ECDH_compute_key(ByVal pout() As Byte, ByVal outlen As Integer, ByVal pub_key As IntPtr, ByVal ecdh As IntPtr, ByVal kdf As ECDH_KDF) As Integer
		//End Function
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDH_compute_key(byte[] pout, int outlen, IntPtr pub_key, IntPtr ecdh, IntPtr kdf);
		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDH_get_ex_new_index(IntPtr argl, IntPtr argp, IntPtr new_func, IntPtr dup_func, IntPtr free_func);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static int ECDH_set_ex_data(IntPtr d, int idx, IntPtr arg);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr ECDH_get_ex_data(IntPtr d, int idx);

		[DllImport(DLLNAME, CallingConvention=CallingConvention.Cdecl)]
		public extern static void ERR_load_ECDH_strings();

#endregion


	}
}