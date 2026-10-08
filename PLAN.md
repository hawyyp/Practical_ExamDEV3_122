# 📋 แผนปฏิบัติงานการสอบปฏิบัติ (Practical Exam Implementation Plan)
**วิชา:** 958321 Game Development 3 (Practical Examination 1/2569)  
**คะแนนรวม:** 75 คะแนน (12 คำถาม 4 หมวด)  
**เป้าหมาย:** พัฒนาตามข้อกำหนดของเอกสาร `169-PracticalExam.pdf` อย่างเคร่งครัด 100% ไม่มีตกหล่น

---

## 🔒 กฎเหล็กและข้อกำหนดที่ต้องยึดถือตลอดการทำงาน
1. **Namespace บังคับ:** Source Code C# ทั้งหมดต้องอยู่ภายใต้ `namespace Ratchanon` ตามชื่อภาษาอังกฤษของนักศึกษาอย่างเคร่งครัด
2. **Git Control:** ทุก Phase ที่สำเร็จจะต้อง Commit ด้วยข้อความที่สื่อความหมายชัดเจน และ Push ขึ้น GitHub `origin/main` ทันที
3. **ProBuilder Only:** ฉากใน Stage 1 และ Stage 2 ต้องสร้างด้วย ProBuilder
4. **ความแม่นยำของโจทย์:** ตรวจสอบเงื่อนไขตัวเลข (เวลา 30s, คะแนนชนะ > 10,000, ไอเท็ม 4 แบบ, ขนาดฉาก Stage 2 $\ge 50$ ตร.ม.) ให้ตรงตาม PDF 100%

---

## 🗺️ แผนการดำเนินงาน 6 ระยะ (Phase-by-Phase Plan)

### Phase 1: Project Setup & Asset Preparation
- [x] 1.1 ติดตั้ง Packages ที่จำเป็น: `com.unity.probuilder` (v6.1.3)
- [x] 1.2 นำเข้าและจัดระเบียบ Assets จาก `PracticalExamResources/` เข้าสู่ `Assets/Resources_Exam/` (Models, Animations, Sounds) พร้อมตั้งค่า Humanoid Rig
- [x] 1.3 Git Commit & Push: `Phase 1: Project setup, installed ProBuilder, configured Git LFS and organized assets` (Commit: `f1a0c7d`)

### Phase 2: Scenes Management & Flow (Q.1 — 10 คะแนน)
- [ ] 2.1 สร้างฉากครบทั้ง 8 ซีนตาม Fig. 1 ใน PDF:
  1. `01_MainMenu`
  2. `02_StageSelect`
  3. `03_Stage1`
  4. `04_Stage2`
  5. `05_Options`
  6. `06_Credits`
  7. `07_Win`
  8. `08_GameOver`
- [ ] 2.2 เพิ่มทุกซีนลงใน Build Settings (Index 0–7)
- [ ] 2.3 ออกแบบ UI และ Label สำหรับแต่ละฉากตามผัง Figma
- [ ] 2.4 สร้าง `SceneController.cs` (ภายใต้ namespace) ควบคุมการเปลี่ยนฉากและส่งต่อข้อมูลคะแนน
- [ ] 2.5 Git Commit & Push: `Phase 2: Completed Scene Management and UI navigation for all 8 scenes (Q.1)`

### Phase 3: Audio Management & Volume Control (Q.9, Q.10 — 10 คะแนน)
- [ ] 3.1 สร้าง AudioMixer (Master, BGM, SFX)
- [ ] 3.2 สร้าง `AudioManager.cs` (Singleton / DontDestroyOnLoad) เล่น BGM ต่อเนื่องทุกฉาก
- [ ] 3.3 ระบบ Volume Control ในหน้า Options (Q.9) ผ่าน UI Sliders
- [ ] 3.4 ตรวจสอบ BGM ในทุกฉาก (Q.10)
- [ ] 3.5 Git Commit & Push: `Phase 3: Implemented AudioMixer, persistent AudioManager, and Volume Control in Options (Q.9, Q.10)`

### Phase 4: Stage 1 — Capsule Player Minigame (Q.2, Q.3, Q.4, Q.5, Q.11, Q.12 — 30 คะแนน)
- [ ] 4.1 Capsule Player Configuration (Q.2): ตัวละคร Capsule, CharacterController, `CapsulePlayerController.cs`, กล้อง Follow
- [ ] 4.2 ProBuilder Level Design Stage 1 (Q.3): ฉากทางกว้าง ทางแคบ ทางเลี้ยว และจุดตกเหว
- [ ] 4.3 ไอเท็มและสิ่งกีดขวาง (Q.4, Q.11, Q.12):
  - ไอเท็ม 4 แบบ: เพิ่มเวลา (+3s), ลดเวลา (-5s), เพิ่มคะแนน (+500), ลดคะแนน (-500)
  - สิ่งกีดขวาง (ชนแล้วเล่น SFX)
- [ ] 4.4 HUD & Game Loop Stage 1 (Q.5): นับถอยหลัง 30 วินาที, แสดงคะแนน, ชนะเมื่อ > 10,000, แพ้เมื่อตกเหวหรือคะแนนไม่ถึง
- [ ] 4.5 Git Commit & Push: `Phase 4: Completed Stage 1 with ProBuilder level, Capsule player, items, and win/lose rules (Q.2-Q.5, Q.11, Q.12)`

### Phase 5: Stage 2 — Humanoid Player Minigame (Q.6, Q.7, Q.8 — 25 คะแนน)
- [ ] 5.1 Humanoid Player Configuration (Q.6): ตัวละคร Humanoid, Animator (Idle, Walk, Run), Third-Person Controller
- [ ] 5.2 ProBuilder Level Design Stage 2 (Q.7): ฉากขนาดไม่ต่ำกว่า 50 ตารางเมตร
- [ ] 5.3 ระบบ Interaction เก็บไอเท็ม Stage 2 (Q.8): กดปุ่มโต้ตอบ (กด E) เพื่อเก็บไอเท็มแทนการเดินชน
- [ ] 5.4 Game Loop & HUD Stage 2: Time Attack 30s, เงื่อนไขคะแนน > 10,000
- [ ] 5.5 Git Commit & Push: `Phase 5: Completed Stage 2 with Humanoid character, 50sqm ProBuilder arena, and E-key interaction system (Q.6-Q.8)`

### Phase 6: Final Verification, Polish & Submission Preparation
- [ ] 6.1 Playtest & Debug ระบบทั้งหมด
- [ ] 6.2 ตรวจสอบ Namespace ใน Source code ทุกไฟล์
- [ ] 6.3 บันทึกผลลัพธ์และเตรียมข้อมูลสรุปหลักฐานสำหรับใส่ใบข้อสอบ PDF ข้อ 1–12
- [ ] 6.4 Final Commit & Push: `Phase 6: Final testing, code polish, and submission verification`
