import React from "react";
import HRMSummary from '../HRMSummary';
import EmployeeInfo from '../EmployeeInfo';
import renderer from 'react-test-renderer';

describe("Test render correctly ", () => {
    it('render hrm detail correctly', () => {
        const hrmDetail = {
            toeicScore: 0,
            toeicExamDate: "0001-01-01T00:00:00",
            missingCheckInCheckOut: 3,
            incident: 0,
            totalLateInEarlyOut: 3
        };
        const component = renderer.create(<HRMSummary hrmDetail={hrmDetail} />);
        expect(component.toJSON()).toMatchSnapshot();

        hrmDetail.toeicScore = 660;
        component.update();
        expect(component.toJSON()).toMatchSnapshot();
    });

    it('Render EmployeeInfo correctly with props', () => {
        const employee = {
            avatar: 'oliver',
            nickName: 'Oliver',
            fullName: 'Đặng Hoàng Hải',
            jobTitle: 'Software Engineer'
        };
        const component = renderer.create(<EmployeeInfo employee={employee} />);
        expect(component.toJSON()).toMatchSnapshot();
    });

    describe('Test for performance appraisal', () => {
        let data = [{
        }];
        it('Component will render correctly', () => {
        });
    });
});