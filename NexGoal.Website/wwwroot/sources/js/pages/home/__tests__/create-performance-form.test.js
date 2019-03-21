import React from 'react';
import { shallow } from 'enzyme';
import CreateSelfPerformance from '../CreateSelfPerformance';
import renderer from 'react-test-renderer';
import Select from 'react-select';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen';
import { Button } from 'react-bootstrap';
import mockAxios from 'axios';

jest.mock('axios');
jest.mock('moment', () => () => ({ format: () => '2018–05–11T12:00:00+00:00' }));
jest.mock('../../../resourceHelper');
const currentUser = { nickName: 'Quinn', avatar: '' };
const projectOptions = [];
const gradeOptions = [];

const props = { currentUser, projectOptions, gradeOptions };
describe('Render', () => {
    it('render correctly match snapshot when form is opening', () => {
        const component = renderer.create(<CreateSelfPerformance {...props} />);
        expect(component.toJSON()).toMatchSnapshot();
    });

    it('render correctly when open form', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        expect(wrapper.find('.form-filter')).toHaveLength(1);
        expect(wrapper.find(Select)).toHaveLength(2);
    });

    it('Do not show form when isOpenForm is false', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: false });
        expect(wrapper.find('.form-filter')).toHaveLength(0);
        expect(wrapper.find('input')).toHaveLength(1);
    });
});

describe('Event tests', () => {
    beforeEach(() => {
        mockAxios.mockImplementation(() =>
            Promise.resolve({
                data: {
                    isSuccessful: true
                }
            })
        );
    });

    afterEach(() => {
        mockAxios.mockReset();
    });

    it('when change form call the right method', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        let event = { target: { value: 'spy' } };
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
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        const spySubmit = jest.spyOn(wrapper.instance(), 'handleSubmit');
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(2).simulate('click', {
            preventDefault: () => {
            }
        });
        expect(spySubmit).toHaveBeenCalledTimes(1);
        expect(spyValidateForm).toHaveBeenCalledTimes(1);
    });

    it('when form validated, axios will be called to submit form to server', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        spyValidateForm.mockReturnValue(true);
        spyOn(document, 'getElementsByName').and.callFake(function () {
            return [{
                value: 'abc test'
            }];
        });
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(2).simulate('click', {
            preventDefault: () => {
            },
            target: {
                name: 'save'
            }
        });
        expect(mockAxios).toHaveBeenCalledTimes(1);
    });

    it('when form not validated, axios will NOT be called to submit form to server', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        const spyValidateForm = jest.spyOn(wrapper.instance(), 'validateForm');
        spyValidateForm.mockReturnValue(false);
        spyOn(document, 'getElementsByName').and.callFake(function () {
            return [{
                value: 'abc test'
            }];
        });
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(2).simulate('click', {
            preventDefault: () => {
            },
            target: {
                name: 'save'
            }
        });
        expect(mockAxios).not.toHaveBeenCalled();
    });

    it('when click Cancel form isclose', () => {
        const wrapper = shallow(<CreateSelfPerformance {...props} />);
        wrapper.setState({ isOpenForm: true });
        const spyCancel = jest.spyOn(wrapper.instance(), 'handleCancel');
        wrapper.instance().forceUpdate();
        wrapper.find(Button).at(0).simulate('click');
        expect(spyCancel).toHaveBeenCalledTimes(1);
        expect(wrapper.find('.form-filter')).toHaveLength(0);
    });
});