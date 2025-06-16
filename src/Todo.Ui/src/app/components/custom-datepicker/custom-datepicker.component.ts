import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'app-custom-datepicker',
    standalone: true,
    imports: [CommonModule],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => CustomDatepickerComponent),
            multi: true
        }
    ],
    template: `
    <div class="relative">
      <input 
        type="date" 
        [value]="value" 
        (input)="onInputChange($event)"
        [disabled]="disabled"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&::-webkit-calendar-picker-indicator]:filter [&::-webkit-calendar-picker-indicator]:dark:invert"
        [id]="inputId"
      >
    </div>
  `,
    styles: [`
    :host {
      display: block;
      width: 100%;
    }
  `]
})
export class CustomDatepickerComponent implements ControlValueAccessor {
    @Input() inputId: string = '';

    value: string = '';
    disabled: boolean = false;

    @Output() valueChange = new EventEmitter<string>();

    private onChange = (value: string) => { };
    private onTouched = () => { };

    onInputChange(event: Event): void {
        const target = event.target as HTMLInputElement;
        this.value = target.value;
        this.onChange(this.value);
        this.onTouched();
        this.valueChange.emit(this.value);
    }

    writeValue(value: string): void {
        this.value = value || '';
    }

    registerOnChange(fn: (value: string) => void): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: () => void): void {
        this.onTouched = fn;
    }

    setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }
} 