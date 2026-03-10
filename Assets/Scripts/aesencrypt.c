#include <openssl/evp.h>
#include <string.h>

int main ()
        {
        /* Allow enough space in output buffer for additional block */
        unsigned char outbuf[1024 + EVP_MAX_BLOCK_LENGTH];
        int inlen, outlen, templen;
        EVP_CIPHER_CTX *ctx;
        /* Bogus key and IV: we'd normally set these from
         * another source.
         */
	unsigned char plaintext [] ="This is a top secret.";
        unsigned char key [] ="10th############";
        unsigned char iv [] ={1,2,3,4,5,6,7,8,9,0,10,11,12,13,14,15};
	unsigned char ciphertext []= {0xe4,0x60,0xba,0x5b,0x5f,0xce,0xde,0xc6,0x5e,0x85,0x56,0xbb,0x59,0x23,0x75,0x37,0x75,0x88,0x1b,0x53,0x83,0x57,0xf3,0x4b,0x13,0xfd,0x1a,0x34,0xe1,0xad,0xaa,0x64};


        /* Don't set key or IV right away; we want to check lengths */
        ctx = EVP_CIPHER_CTX_new();
        EVP_CipherInit_ex(ctx, EVP_aes_128_cbc(), NULL, NULL, NULL,1);
        OPENSSL_assert(EVP_CIPHER_CTX_key_length(ctx) == 16);
        OPENSSL_assert(EVP_CIPHER_CTX_iv_length(ctx) == 16);

        /* Now we can set key and IV */
        EVP_CipherInit_ex(ctx, EVP_aes_128_cbc(), NULL, key, iv,1);

        if (!EVP_EncryptUpdate(ctx, outbuf, &outlen, plaintext, strlen(plaintext))) {
         /* Error */
         EVP_CIPHER_CTX_free(ctx);
         return 0;
     	}
	if (!EVP_EncryptFinal_ex(ctx, outbuf+outlen, &templen)) {
         /* Error */
         EVP_CIPHER_CTX_free(ctx);
         return 0;
     	}
	

	printf("plaintextlength: %d\t ciphertext length:%d\n", strlen(plaintext), outlen+templen);
	for (int i=0;i<outlen+templen;i++){
	 printf("%x", outbuf[i]);
	}
	
	
	printf("\n");
	if (memcmp(ciphertext, outbuf, 32) == 0)
	printf("ciphertext matched!\n");

        EVP_CIPHER_CTX_cleanup(ctx);
        return 1;
        }