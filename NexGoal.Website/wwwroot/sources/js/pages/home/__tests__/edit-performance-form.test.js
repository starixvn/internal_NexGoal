import React from "react";
import { shallow, mount } from "enzyme";
import EditSelfPerformance from '../EditSelfPerformance';
import renderer from 'react-test-renderer';
import Select from 'react-select';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen';
import { Button } from 'react-bootstrap';
import mockAxios from "axios";

jest.mock('axios');
jest.mock('moment', () => () => ({ format: () => '2018–05–11T12:00:00+00:00' }));
jest.mock('../../../resourceHelper');
const currentUser = { nickName: 'Quinn', avatar: '' };
const projectOptions = [];
const gradeOptions = [];

const props = { currentUser, projectOptions, gradeOptions };
const record = {
    comment: "aaa",
    createdDate: "2018-10-31T08:38:17.77",
    creator: { employeeId: 363, code: "02469", nickName: "Oliver", fullName: "Đặng Hoàng Hải", groupCode: "NEX" },
    employees: [],
    grade: { value: 4, label: "NI+" },
    isHost: true,
    modifiedDate: "2018-10-31T08:39:41.26",
    performanceRecordId: 91,
    roleId: 0,
    projects: [{ value: 11, label: "NAP - Special Delete" }],
    statusId: 3
};

describe('When event have been did mount', () => {
    beforeEach(() => {
        mockAxios.mockImplementation(() =>
            Promise.resolve({
                data: record
            })
        );
    });

    afterEach(() => {
        mockAxios.mockReset();
    });

    it('When did mount, component should call axios', () => {
        const spy = jest.spyOn(EditSelfPerformance.prototype, 'componentDidMount');
        const wrapper = mount(<EditSelfPerformance {...props} />);
        expect(spy).toHaveBeenCalled();
        expect(mockAxios).toHaveBeenCalledTimes(1);
    });

    it('render correctly match snapshot when render', () => {
        const component = renderer.create(<EditSelfPerformance {...props} />);
        expect(component.toJSON()).toMatchSnapshot();
    });
});

describe('Event tests', () => {
    beforeEach(() => {
        mockAxios.mockImplementation(() =>
            Promise.resolve({
                data: {
                    isSuccessful: true,
                    data: record
                }
            })
        );
    });

    afterEach(() => {
        mockAxios.mockReset();
    });

    it('when change form call the right method', () => {
        const wrapper = shallow(<EditSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        let event = { target: { value: "spy" } };
        const spyGradeChange = jest.spyOn(wrapper.instance(), 'handleGradeChange');
        wrapper.instance().forceUpdate();
        wrapper.find(Select).first().simulate('change', event);
        expect(spyGradeChange).toHaveBeenCalled();

        const spyProjectsChange = jest.spyOn(wrapper.instance(), 'handleProjectChange');
        wrapper.instance().forceUpdate();
        wrapper.find(Select).at(1).simulate('change', event);
        expect(spyProjectsChange).toHaveBeenCalled();

        const spyCommentOnChange = jest.spyOn(wrapper.instance(), 'handleCommentChange');
        wrapper.instance().forceUpdate();
        wrapper.find(TextareaFullscreenEditor).simulate('change', event);
        expect(spyCommentOnChange).toHaveBeenCalled();
    });

    it('when submit form will call validateForm', () => {
        const wrapper = shallow(<EditSelfPerformance {...props} />);
        const spySubmit = jest.spyOn(wrapper.instance(), 'handleSubmit');
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(1).simulate('click', {
            preventDefault: () => {
            }
        });
        expect(spySubmit).toHaveBeenCalledTimes(1);
        expect(spyValidateForm).toHaveBeenCalledTimes(1);
    });

    it('when form validated, axios will be called to submit form to server', () => {
        const wrapper = shallow(<EditSelfPerformance {...props} />);
        wrapper.setState({ performanceRecord: record });
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        spyValidateForm.mockReturnValue(true);
        spyOn(document, "getElementsByName").and.callFake(function () {
            return [{
                value: "abc test"
            }];
        });
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(1).simulate('click', {
            preventDefault: () => {
            },
            target: {
                name: 'save'
            }
        });
        expect(mockAxios).toHaveBeenCalledTimes(2);
    });

    it('when form not validated, axios will NOT be called to submit form to server', () => {
        const wrapper = shallow(<EditSelfPerformance {...props} />);
        wrapper.setState({ performanceRecord: record });
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        spyValidateForm.mockReturnValue(false);
        spyOn(document, "getElementsByName").and.callFake(function () {
            return [{
                value: "abc test"
            }];
        });
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(1).simulate('click', {
            preventDefault: () => {
            },
            target: {
                name: 'save'
            }
        });
        expect(mockAxios).toHaveBeenCalledTimes(1);
    });
});