import os
import re
import pandas as pd


def extract_alerts_from_excel(excel_path):
    """讀取單一 Excel 檔案並解析，完全依照 Excel 畫面上的上下順序，絕不自行重排"""
    if not os.path.exists(excel_path):
        print(f"⚠️ 找不到檔案：{excel_path}，跳過處理。")
        return []

    print(f"📖 正在讀取：{excel_path}...")

    # 1. 讀取 Excel 檔案，遇到空活頁簿或壞檔直接跳過
    try:
        df_raw = pd.read_excel(excel_path)
    except Exception as e:
        print(
            f"❌ 警告：檔案 {excel_path} 無法讀取（可能損壞或無工作表）。錯誤訊息: {e}。已跳過該檔案。"
        )
        return []

    # 2. 動態尋找包含「软元件」的正確表頭列
    header_row_idx = None
    for idx, row in df_raw.iterrows():
        if "软元件" in row.values:
            header_row_idx = idx
            break

    if header_row_idx is None:
        print(
            f"❌ 錯誤：在 {excel_path} 中找不到 '软元件' 表頭，跳過此檔案。"
        )
        return []

    # 3. 重新載入正式資料
    df = pd.read_excel(excel_path, skiprows=header_row_idx + 1)

    parsed_alerts = []
    # Regex：精準抓取 D7002 與 .b 後面的數字/文字
    pattern = re.compile(r"([A-Z]+\d+)\.b([0-9A-Fa-f]+)")

    for _, row in df.iterrows():
        # 如果點位或描述完全是空的，直接跳過
        if pd.isna(row.get("软元件")) or pd.isna(row.get("描述")):
            continue

        device_str = str(row["软元件"]).strip()
        description = str(row["描述"]).strip()

        # 處理 ALID (基本报警注释号)，如果遇到 '??' 或空白，原樣保留字串
        alid = (
            str(row["基本报警注释号"]).strip()
            if pd.notna(row.get("基本报警注释号"))
            else "0"
        )

        match = pattern.search(device_str)
        if match:
            word = match.group(1)
            bit_raw = match.group(2)

            # 🟢 直覺解析：如果是純數字（如 0~11），就直接當作字串輸出
            # 如果是 A~F 這種點位，才轉成 10 進位數字
            if bit_raw.isdigit():
                bit = bit_raw
            else:
                try:
                    bit = str(int(bit_raw, 16))
                except ValueError:
                    bit = bit_raw  # 失敗則維持原樣

            # 轉義描述中的雙引號
            description_escaped = description.replace('"', '\\"')

            # 直接依序塞入，不經過任何排序操作
            parsed_alerts.append((word, bit, alid, description_escaped))

    print(f"✅ {excel_path} 解析完成，共 {len(parsed_alerts)} 筆點位。")
    return parsed_alerts


def generate_combined_csharp_code(file_list, output_path="output_code.txt"):
    all_csharp_lines = []
    code_index = 0

    # 依照你的檔案順序，一個檔案讀完接著讀下一個
    for excel_file in file_list:
        alerts = extract_alerts_from_excel(excel_file)

        # 這裡的 alerts 完全保留了 Excel 從上到下的物理順序
        for word, bit, alid, description in alerts:
            line = f'_alertTotalList[{code_index}] = new string[] {{ "{word}", "{bit}", "{alid}", "{description}" }};'
            all_csharp_lines.append(line)
            code_index += 1

    with open(output_path, "w", encoding="utf-8") as f:
        f.write("\n".join(all_csharp_lines))

    print("\n" + "=" * 50)
    print(f"✨ 全部轉換完成！")
    print(f"📦 總共依照原始視覺順序生成 {code_index} 筆 C# 警報程式碼。")
    print(f"💾 結果已儲存至：{output_path}")


if __name__ == "__main__":
    # 這裡的陣列順序決定了哪份檔案的警報排在前面
    target_excels = ["警报.xlsx", "SE报警触发.xlsx"]
    generate_combined_csharp_code(target_excels)