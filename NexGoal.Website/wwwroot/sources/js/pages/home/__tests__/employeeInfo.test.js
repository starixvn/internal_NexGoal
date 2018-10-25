import React from "react";
import EmployeeInfo from '../EmployeeInfo';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';

describe("Render correctly ", () => {
    it('Render correctly with props', () => {
        const employee = {
            avatar: 'oliver',
            nickName: 'Oliver',
            fullName: 'Đặng Hoàng Hải',
            jobTitle: 'Software Engineer'
        };
        const component = renderer.create(<EmployeeInfo employee={employee} />);
        expect(component.toJSON()).toMatchSnapshot();
    });
});