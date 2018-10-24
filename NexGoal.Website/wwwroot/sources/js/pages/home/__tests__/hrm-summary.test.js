import React from "react";
import { mount } from "enzyme";
import HRMSummary from '../HRMSummary';
import renderer from 'react-test-renderer';
import axios from "axios";

jest.mock('axios');

const data = {
    performanceRecordId: 999,
    gradeId: 2,
    grade: { name: '' },
    comment: '',
    createdDate: '',
    employee: { nickName: 'Oliver' },
    creator: { nickName: 'Michael', avatar: '' }
};

describe("Test render correctly ", () => {
    beforeEach(() => {
        axios.get.mockImplementation(() =>
            Promise.resolve({
                data: data
            })
        );
    });

    afterEach(() => {
        axios.get.mockReset();
    });

    it("should fetch data when mount", async () => {
        const wrapper = mount(<HRMSummary />);
        expect(axios.get).toHaveBeenCalledTimes(1);
    });

    it('renderUI correctly', () => {
        const component = renderer.create(<HRMSummary />);
        expect(component.toJSON()).toMatchSnapshot();
    });
});