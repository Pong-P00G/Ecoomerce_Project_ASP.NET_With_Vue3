<script setup>
import { useRouter } from 'vue-router';

const router = useRouter();
</script>

<template>
  <div class="min-h-screen bg-[#f9fafb] relative px-4 py-16">
    <!-- dotted background -->
    <div class="absolute inset-0 bg-[radial-gradient(#e5e7eb_1px,transparent_1px)] [background-size:18px_18px]"></div>

    <div class="relative mx-auto max-w-3xl">
      <!-- Header -->
      <div class="mb-10 text-center">
        <h1 class="text-3xl font-semibold text-gray-900">
          Terms of Protocol & Privacy Directives
        </h1>
        <p class="mt-2 text-sm text-gray-500">
          Effective Date: {{ new Date().getFullYear() }}
        </p>
      </div>

      <!-- Card -->
      <div class="rounded-2xl bg-white p-8 shadow-lg space-y-10">

        <!-- TERMS -->
        <section>
          <h2 class="text-xl font-semibold text-gray-900 mb-4">
            1. Terms of Protocol
          </h2>

          <div class="space-y-4 text-sm text-gray-600 leading-relaxed">
            <p>
              By creating an account or accessing our platform, you agree to
              comply with these Terms of Protocol. These terms govern your use
              of our services, applications, and infrastructure.
            </p>

            <ul class="list-disc pl-5 space-y-2">
              <li>You must provide accurate and complete registration information.</li>
              <li>You are responsible for maintaining the confidentiality of your credentials.</li>
              <li>Unauthorized access, abuse, or misuse of the system is prohibited.</li>
              <li>We reserve the right to suspend or terminate accounts violating these terms.</li>
            </ul>

            <p>
              The platform may evolve over time. We may update features,
              protocols, or policies to improve security and performance.
            </p>
          </div>
        </section>

        <!-- PRIVACY -->
        <section>
          <h2 class="text-xl font-semibold text-gray-900 mb-4">
            2. Privacy Directives
          </h2>

          <div class="space-y-4 text-sm text-gray-600 leading-relaxed">
            <p>
              Your privacy is important to us. This section explains how we
              collect, use, and protect your personal information.
            </p>

            <ul class="list-disc pl-5 space-y-2">
              <li>We collect basic account data such as name, email, and login activity.</li>
              <li>Passwords are securely hashed and never stored in plain text.</li>
              <li>Authentication tokens are stored securely based on your session preference.</li>
              <li>We do not sell or share personal data with third parties without consent.</li>
            </ul>

            <p>
              You may request data access, correction, or deletion by contacting
              our support team.
            </p>
          </div>
        </section>

        <!-- COOKIES -->
        <section>
          <h2 class="text-xl font-semibold text-gray-900 mb-4">
            3. Cookies & Sessions
          </h2>

          <p class="text-sm text-gray-600 leading-relaxed">
            We use cookies or session storage to maintain authentication state,
            enhance security, and improve user experience. You may disable
            cookies in your browser, but some features may not function properly.
          </p>
        </section>

        <!-- ACCEPTANCE -->
        <section>
          <h2 class="text-xl font-semibold text-gray-900 mb-4">
            4. Acceptance
          </h2>

          <p class="text-sm text-gray-600 leading-relaxed">
            By registering an account, you acknowledge that you have read,
            understood, and agreed to these Terms of Protocol and Privacy
            Directives.
          </p>
        </section>

        <!-- ACTION -->
        <div class="pt-6 border-t border-gray-200 text-center">
          <button
              @click="router.back()"
              class="inline-flex items-center gap-2 rounded-xl bg-gray-900 px-6 py-3 text-sm font-semibold text-white hover:bg-gray-800"
          >
            Back
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
