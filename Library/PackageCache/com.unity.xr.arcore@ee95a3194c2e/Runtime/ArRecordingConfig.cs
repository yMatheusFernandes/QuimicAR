using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// A recording configuration struct that contains the configuration for session recording.
    /// See <see cref="ArSession.StartRecording(ArRecordingConfig)"/>.
    /// </summary>
    /// <example>
    /// <code source="../Tests/Runtime/CodeExamples.cs" region="ArRecordingConfig_example"/>
    /// </example>
    /// <remarks>
    /// A <see cref="ArRecordingConfig"/> represents a native object that must be disposed (by calling
    /// <see cref="ArRecordingConfig.Dispose"/>) to prevent memory leaks. Consider using a `using` statement for
    /// convenience.
    ///
    /// This is a C# wrapper for ARCore's
    /// [native ArRecordingConfig](https://developers.google.com/ar/reference/c/group/ar-recording-config)
    /// </remarks>
    public struct ArRecordingConfig : IEquatable<ArRecordingConfig>, IDisposable
    {
        IntPtr m_Self;

        ArRecordingConfig(IntPtr value) => m_Self = value;

        /// <summary>
        /// Create an instance from a native pointer. The native pointer must point to an existing <see cref="ArRecordingConfig"/>.
        /// </summary>
        /// <param name="value">A pointer to an existing native <see cref="ArRecordingConfig"/>.</param>
        /// <returns>An instance whose native pointer is <paramref name="value"/>.</returns>
        public static ArRecordingConfig FromIntPtr(IntPtr value) => new ArRecordingConfig(value);

        /// <summary>
        /// Gets the native pointer for this instance.
        /// </summary>
        /// <returns>The native pointer.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Casts an instance to its native pointer.
        /// </summary>
        /// <param name="config">The instance to cast.</param>
        /// <returns>The native pointer.</returns>
        public static explicit operator IntPtr(ArRecordingConfig config) => config.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two instances are considered equal if their native pointers are equal.
        /// </remarks>
        /// <param name="other">The instance to compare against.</param>
        /// <returns><see langword="true"/> if the native pointers are equal. Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(ArRecordingConfig other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An `object` to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an `ArRecordingConfig` and it compares
        /// equal to this instance using <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.</returns>
        public override bool Equals(object obj) => obj is ArRecordingConfig other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>Returns a hash code for this instance.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(ArRecordingConfig lhs, ArRecordingConfig rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(ArRecordingConfig lhs, ArRecordingConfig rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you compare an instance with <see langword="null"/> to determine whether its
        /// native pointer is null.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArRecordingConfig obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArRecordingConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArRecordingConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="false"/>
        /// </returns>
        public static bool operator ==(ArRecordingConfig? lhs, ArRecordingConfig? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you compare an instance with <see langword="null"/> to determine whether its
        /// native pointer is `null`.
        /// </remarks>
        /// <example>
        /// <code>
        /// bool TestForNull(ArRecordingConfig obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="lhs">The nullable `ArRecordingConfig` to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable `ArRecordingConfig` to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not `null` and their native pointers are equal.
        /// - <paramref name="lhs"/> is `null` and <paramref name="rhs"/>'s native pointer is `null`.
        /// - <paramref name="rhs"/> is `null` and <paramref name="lhs"/>'s native pointer is `null`.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are `null`.
        ///
        /// Otherwise, returns <see langword="true"/>.
        /// </returns>
        public static bool operator !=(ArRecordingConfig? lhs, ArRecordingConfig? rhs) => !(lhs == rhs);

        /// <summary>
        /// Creates a dataset recording config object. This object must be disposed with <see cref="Dispose"/>.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        public ArRecordingConfig(ArSession session) => Create(session, out this);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_create")]
        static extern void Create(ArSession session, out ArRecordingConfig outConfig);

        /// <summary>
        /// Releases memory used by this recording config object.
        /// </summary>
        public void Dispose()
        {
            if (this != null)
            {
                Destroy(this);
            }

            m_Self = IntPtr.Zero;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_destroy")]
        static extern void Destroy(ArRecordingConfig config);

        /// <summary>
        /// Gets the file path to save an MP4 dataset file for this recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the path to the MP4 dataset file to which the recording should be saved.</returns>
        [Obsolete("GetMp4DatasetFilePath is deprecated in AR Foundation 6.1. Use GetMp4DatasetUri(ArSession session) instead")]
        public string GetMp4DatasetFilePath(ArSession session)
        {
            GetMp4DatasetFilePath(session, this, out var value);
            using (value)
            {
                return value.ToString(Encoding.UTF8);
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getMp4DatasetFilePath")]
        static extern void GetMp4DatasetFilePath(ArSession session, ArRecordingConfig config, out ArString outMp4DatasetFilePath);

        /// <summary>
        /// Gets the URI to save an MP4 dataset file for this recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the URI to the MP4 dataset to which the recording should be saved.</returns>
        public string GetMp4DatasetUri(ArSession session)
        {
            GetMp4DatasetUri(session, this, out var value);
            using (value)
            {
                return value.ToString(Encoding.UTF8);
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getMp4DatasetUri")]
        static extern void GetMp4DatasetUri(ArSession session, ArRecordingConfig config, out ArString outMp4DatasetUri);

        /// <summary>
        /// Sets the file path to save an MP4 dataset file for the recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="path">The file path to which an MP4 dataset should be written.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path"/> is `null`.</exception>
        [Obsolete("SetMp4DatasetFilePath is deprecated in AR Foundation 6.1. Use SetMp4DatasetUri(ArSession session, string uri) instead")]
        public void SetMp4DatasetFilePath(ArSession session, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            unsafe
            {
                using (var bytes = path.ToBytes(Encoding.UTF8, Allocator.Temp))
                {
                    SetMp4DatasetFilePath(session, this, (byte*)bytes.GetUnsafePtr());
                }
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setMp4DatasetFilePath")]
        static extern unsafe void SetMp4DatasetFilePath(ArSession session, ArRecordingConfig config, byte* mp4DatasetFilePath);

        /// <summary>
        /// Sets the URI to save an MP4 dataset file for the recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="uri">The URI to which an MP4 dataset should be written.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="uri"/> is `null`.</exception>
        public void SetMp4DatasetUri(ArSession session, string uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            unsafe
            {
                using (var bytes = uri.ToBytes(Encoding.UTF8, Allocator.Temp))
                {
                    SetMp4DatasetUri(session, this, (byte*)bytes.GetUnsafePtr());
                }
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setMp4DatasetUri")]
        static extern unsafe void SetMp4DatasetUri(ArSession session, ArRecordingConfig config, byte* mp4DatasetUri);

        /// <summary>
        /// Gets the setting that indicates whether this recording should stop automatically when the ARCore session is paused.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns `true` if this recording should stop when the ARCore session is paused. Returns `false` otherwise.</returns>
        public bool GetAutoStopOnPause(ArSession session)
        {
            GetAutoStopOnPause(session, this, out var value);
            return value != 0;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getAutoStopOnPause")]
        static extern void GetAutoStopOnPause(ArSession session, ArRecordingConfig config, out int outConfigEnabled);

        /// <summary>
        /// Sets whether this recording should stop automatically when the ARCore session is paused.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="value">If `true`, this recording will stop automatically when the ARCore session is paused. If `false`, the recording will continue.</param>
        public void SetAutoStopOnPause(ArSession session, bool value) => SetAutoStopOnPause(session, this, value ? 1 : 0);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setAutoStopOnPause")]
        static extern void SetAutoStopOnPause(ArSession session, ArRecordingConfig config, int configEnabled);

        /// <summary>
        /// Gets the clockwise rotation in degrees that should be applied to the recorded image.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the rotation in degrees that will be applied to the recorded image. Possible values are 0, 90, 180, 270, or -1 if unspecified.</returns>
        public int GetRecordingRotation(ArSession session)
        {
            GetRecordingRotation(session, this, out var value);
            return value;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getRecordingRotation")]
        static extern void GetRecordingRotation(ArSession session, ArRecordingConfig config, out int outRecordingRotation);

        /// <summary>
        /// Specifies the clockwise rotation in degrees that should be applied to the recorded image.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="value">The clockwise rotation in degrees (0, 90, 180, or 270).</param>
        public void SetRecordingRotation(ArSession session, int value) => SetRecordingRotation(session, this, value);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setRecordingRotation")]
        static extern void SetRecordingRotation(ArSession session, ArRecordingConfig config, int recordingRotation);
    }
}
