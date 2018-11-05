import React from "react";
import HRMSummary from '../HRMSummary';
import EmployeeInfo from '../EmployeeInfo';
import renderer from 'react-test-renderer';
import PerformanceAppraisalResult from '../PerformanceAppraisalResult';
import { shallow, mount } from 'enzyme';

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
        const data = [{
            employeeId: 10,
            gradeId: "",
            paRecordId: 5,
            period: {
                periodId: 6,
                name: "1h 2018",
                startDate: "2018-08-30T00:00:00",
                endDate: "2018-09-11T00:00:00",
                focusStartDate: null
            },
            periodId: 6,
            statusId: 0,
            year: 2018
        }];

        it('Component will render correctly', () => {
            const component = renderer.create(<PerformanceAppraisalResult performanceAppraisals={data} />);
            expect(component.toJSON()).toMatchSnapshot();
        });

        it('component will call groupPerformanceAppraisals method when render', () => {
            const wrapper = shallow(<PerformanceAppraisalResult performanceAppraisals={data} />);
            const instance = wrapper.instance();
            const spy = jest.spyOn(instance, 'groupPerformanceAppraisals');
            instance.forceUpdate();
            expect(spy).toHaveBeenCalledTimes(1);
        });

        it('shouldComponentUpdate will return right value', () => {
            const wrapper = shallow(<PerformanceAppraisalResult performanceAppraisals={data} />);
            let nextProps;
            let shouldUpdate;

            nextProps = { performanceAppraisals: data };
            shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(false);

            nextProps = {
                performanceAppraisals: []
            };
            shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(true);
        });
    });
});